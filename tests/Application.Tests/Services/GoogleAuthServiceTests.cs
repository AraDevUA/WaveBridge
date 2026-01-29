using Application.Dto.Options;
using Application.Dto.Options.Auth.Google;
using Application.Dto.Response.Auth;
using Application.Dto.Responses.Auth;
using Application.Dto.Responses.Auth.Google;
using Application.Helpers;
using Application.Helpers.Contracts;
using Application.Providers.Contracts;
using Application.Services;
using Application.Tests.Providers;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
namespace Application.Tests.Services;

public class GoogleAuthServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContext;
    private readonly Mock<IHttpClientHelper> _httpClientHelper;
    private readonly Mock<IRepository<UserOAuthConnection, Guid>> _userOAuthConnectionRepository;
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IJwtProvider> _jwtProvider;
    const string testKeyBase64 = "z2z1k8E1o2d9qR0YxHq0G2R9Wm8s4Q9k3Jr1f9c8L1A=";

    public readonly GoogleAuthService _googleAuthService;
    public GoogleAuthServiceTests()
    {
        var storeMock = new Mock<IUserStore<User>>();

        _userManager = new Mock<UserManager<User>>(
            storeMock.Object,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(),
            new IUserValidator<User>[0],
            new IPasswordValidator<User>[0],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            null
        );

        var sessionItems = new Dictionary<string, byte[]>();
        var sessionMock = new Mock<ISession>();
        sessionMock.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                   .Callback<string, byte[]>((key, value) => sessionItems[key] = value);

        sessionMock.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                   .Returns((string key, out byte[]? value) =>
                   {
                       if (sessionItems.TryGetValue(key, out var val))
                       {
                           value = val;
                           return true;
                       }
                       value = null;
                       return false;
                   });

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Session).Returns(sessionMock.Object);

        _httpContext = new Mock<IHttpContextAccessor>();
        _httpContext.Setup(a => a.HttpContext).Returns(contextMock.Object);

        _httpClientHelper = new Mock<IHttpClientHelper>();
        _userOAuthConnectionRepository = new Mock<IRepository<UserOAuthConnection, Guid>>();
        _jwtProvider = new Mock<IJwtProvider>();

        var googleOptions = Options.Create(new GoogleAuthOptions
        {
            CallbackUri = "https://localhost/callback",
            Scopes = new GoogleScopesOptions
            {
                UserInfoScope = "email profile"
            },
            Endpoints = new GoogleEndpointsOptions
            {
                OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth",
                TokenEndpoint = "https://oauth2.googleapis.com/token",
                UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo"
            },
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        });

        var encryptionOptions = Options.Create(new EncryptionOptions
        {
            KeyBase64 = testKeyBase64
        });

        _googleAuthService = new GoogleAuthService(
            googleOptions,
            encryptionOptions,
            _httpContext.Object,
            _httpClientHelper.Object,
            _userOAuthConnectionRepository.Object,
            _userManager.Object,
            _jwtProvider.Object
        );
    }
    #region RedirectOnOAuthServerAsync Tests
    [Fact]
    public async Task RedirectOnOAuthServerAsync_ValidRequest_ReturnsRedirectUrString()
    {
        // Act
        var result = await _googleAuthService.RedirectOnOAuthServerAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith("https://accounts.google.com/o/oauth2/v2/auth?");
    }
    #endregion

    #region RefreshAccessTokenAsync Tests
    [Fact]
    public async Task RefreshAccessTokenAsync_ValidConnection_UpdatesTokens()
    {

        // Arrange
        var encryptedRefreshToken = AesGcmHelper.Encrypt("refresh-token", testKeyBase64);
        var userOAuthConnection = new UserOAuthConnection
        {
            RefreshToken = encryptedRefreshToken
        };
        var tokenResponse = new GoogleTokenResponseDto
        {
            AccessToken = "new-access-token",
            ExpiresIn = 3600,
        };

        _httpClientHelper
            .Setup(h => h.SendPostFormRequestAsync<GoogleTokenResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _userOAuthConnectionRepository
            .Setup(r => r.UpdateAsync(userOAuthConnection, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _googleAuthService.RefreshAccessTokenAsync(userOAuthConnection);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");

        userOAuthConnection.AccessToken.Should().Be("new-access-token");
        userOAuthConnection.AccessTokenExpiresAtUtc.Should().BeAfter(DateTimeOffset.UtcNow);

        _userOAuthConnectionRepository.Verify(
            r => r.UpdateAsync(userOAuthConnection, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_NullConnection_ThrowsInvalidOperationException()
    {
        // Act
        Func<Task> act = async () => await _googleAuthService.RefreshAccessTokenAsync(null!);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    #endregion

    #region OAuthCallbackAsync Tests
    [Fact]
    public async Task OAuthCallbackAsync_UserExists_UpdatesConnectionAndReturnsTokens()
    {
        // Arrange
        var code = "test-code";
        var codeVerifier = "verifier";
        _httpContext.Object.HttpContext.Session.SetString("codeVerifier", codeVerifier);

        var tokenResponse = new GoogleTokenResponseDto
        {
            AccessToken = "access-token",
            ExpiresIn = 3600,
            RefreshToken = "refresh-token"
        };

        var googleUser = new GoogleUserInfoResponseDto
        {
            Email = "existing@test.com",
            Name = "Existing User"
        };

        var existingUser = new User { Id = Guid.NewGuid(), Email = googleUser.Email };

        _httpClientHelper
            .Setup(h => h.SendPostFormRequestAsync<GoogleTokenResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _httpClientHelper
            .Setup(h => h.SendGetRequestAsync<GoogleUserInfoResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);

        _userManager.Setup(u => u.FindByEmailAsync(googleUser.Email))
            .ReturnsAsync(existingUser);

        var connections = new List<UserOAuthConnection>
        {
            new UserOAuthConnection
            {
            UserId = existingUser.Id,
            AccessToken = "old-token",
            AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow
            }
        };


        var asyncConnections = new TestAsyncEnumerable<UserOAuthConnection>(connections);

        _userOAuthConnectionRepository
            .Setup(r => r.All)
            .Returns(asyncConnections);

        _jwtProvider.Setup(j => j.GenerateTokensAsync(existingUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto { Token = "jwt-access", RefreshToken = "jwt-refresh" });

        // Act
        var result = await _googleAuthService.OAuthCallbackAsync(code);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        var tokens = result.Data as AuthResponseDto;
        tokens!.Token.Should().Be("jwt-access");

        _userOAuthConnectionRepository.Verify(
            r => r.UpdateAsync(It.IsAny<UserOAuthConnection>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task OAuthCallbackAsync_NewUser_CreatesUserAndConnection_ReturnsTokens()
    {
        // Arrange
        var code = "test-code";
        _httpContext.Object.HttpContext.Session.SetString("codeVerifier", "verifier");

        var tokenResponse = new GoogleTokenResponseDto
        {
            AccessToken = "access-token",
            ExpiresIn = 3600,
            RefreshToken = "refresh-token"
        };

        var googleUser = new GoogleUserInfoResponseDto
        {
            Email = "test@test.com",
            Name = "Test User"
        };

        _httpClientHelper
            .Setup(h => h.SendPostFormRequestAsync<GoogleTokenResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _httpClientHelper
            .Setup(h => h.SendGetRequestAsync<GoogleUserInfoResponseDto>(It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>?>(),
                It.IsAny<string?>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);

        _userManager.Setup(u => u.FindByEmailAsync(googleUser.Email))
            .ReturnsAsync((User?)null);
        _userManager.Setup(u => u.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _userOAuthConnectionRepository.Setup(r => r.All)
            .Returns(new TestAsyncEnumerable<UserOAuthConnection>(new List<UserOAuthConnection>()));
        _userOAuthConnectionRepository.Setup(r => r.CreateAsync(It.IsAny<UserOAuthConnection>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _jwtProvider.Setup(j => j.GenerateTokensAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new AuthResponseDto { Token = "jwt-new-access", RefreshToken = "jwt-new-refresh" });
        // Act
        var result = await _googleAuthService.OAuthCallbackAsync(code);
        // Assert
        result.IsSuccess.Should().BeTrue();
        var tokens = result.Data as AuthResponseDto;
        tokens!.Token.Should().Be("jwt-new-access");

        _userManager.Verify(u => u.CreateAsync(It.IsAny<User>()), Times.Once);
        _userOAuthConnectionRepository.Verify(
            r => r.CreateAsync(It.IsAny<UserOAuthConnection>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task OAuthCallbackAsync_NewUser_CreationFails_ReturnsFailedResult()
    {
        // Arrange
        var code = "test-code";
        _httpContext.Object.HttpContext.Session.SetString("codeVerifier", "verifier");

        var tokenResponse = new GoogleTokenResponseDto
        {
            AccessToken = "access-token",
            ExpiresIn = 3600,
            RefreshToken = "refresh-token"
        };

        var googleUser = new GoogleUserInfoResponseDto
        {
            Email = "fail@test.com",
            Name = "Fail User"
        };

        _httpClientHelper.Setup(h => h.SendPostFormRequestAsync<GoogleTokenResponseDto>(
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, string>>(),
            It.IsAny<string?>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _httpClientHelper.Setup(h => h.SendGetRequestAsync<GoogleUserInfoResponseDto>(
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, string>?>(),
            It.IsAny<string?>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(googleUser);


        _userManager.Setup(u => u.FindByEmailAsync(googleUser.Email))
            .ReturnsAsync((User?)null);

        _userManager.Setup(u => u.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));
        //act
        var result = await _googleAuthService.OAuthCallbackAsync(code);
        // Assert
        result.IsSuccess.Should().BeFalse();
    }
    #endregion

}

