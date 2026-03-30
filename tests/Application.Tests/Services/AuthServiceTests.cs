using Application.Dto.DtoExtensions;
using Application.Dto.Jwt;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Dto.Response.Users;
using Application.Providers.Contracts;
using Application.Services;
using Domain.Entities;
using Shared.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class AuthServiceTests
{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<IJwtProvider> _jwtProviderMock;
    private AuthService _authService;


    public AuthServiceTests()
    {
        var storeMock = new Mock<IUserStore<User>>();

        _userManagerMock = new Mock<UserManager<User>>(
        storeMock.Object,
        Options.Create(new IdentityOptions()),   // optionsAccessor
        new PasswordHasher<User>(),             // passwordHasher
        new IUserValidator<User>[0],           // userValidators
        new IPasswordValidator<User>[0],       // passwordValidators
        new UpperInvariantLookupNormalizer(),  // keyNormalizer
        new IdentityErrorDescriber(),          // errors
        null,                                  // services
        null                                   // logger
        );

        _jwtProviderMock = new Mock<IJwtProvider>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _jwtProviderMock.Object,
            null!,
            Options.Create(new JwtOptions())
        );
    }

    #region LoginAsync Tests
    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        _userManagerMock
            .Setup(um => um.FindByEmailAsync("test@example.com"))
            .ReturnsAsync((User?)null);

        var dto = new LoginRequestDto { Email = "test@example.com", Password = "123" };
        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.Unauthorized);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, "wrongpassword")).ReturnsAsync(false);
        var dto = new LoginRequestDto { Email = user.Email, Password = "wrongpass" };

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.Unauthorized);
    }
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        var roles = new[] { "User" };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, "123"))
            .ReturnsAsync(true);

        _jwtProviderMock
            .Setup(x => x.GenerateTokensAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "access",
                RefreshToken = "refresh",
                User = user.ToDto(new[] { "User" })
            });

        var dto = new LoginRequestDto { Email = user.Email, Password = "123" };
        // Act
        var result = await _authService.LoginAsync(dto);
        // Assert
        result.Type.Should().Be(ServiceResultType.Ok);
        result.Data.Should().BeEquivalentTo(new AuthResponseDto
        {
            Token = "access",
            RefreshToken = "refresh",
            User = user.ToDto(new[] { "User" })
        });

    }
    #endregion

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_FailedCreate_ReturnsFailed()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "123456"
        };

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError()));
        // Act
        var result = await _authService.RegisterAsync(dto);
        // Assert
        result.Type.Should().Be(ServiceResultType.Conflict);
    }
    [Fact]
    public async Task RegisterAsync_Success_ReturnsOkWithUserDto()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "123456"
        };

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.Ok);
        result.Data.Should().NotBeNull();

        var userDto = result.Data.Should().BeAssignableTo<UserResponseDto>().Subject;
        userDto.Email.Should().Be(dto.Email);
    }
    #endregion

    #region RefreshAccessTokenAsync Tests
    [Fact]
    public async Task RefreshAccessTokenAsync_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        _jwtProviderMock
            .Setup(jp => jp.RefreshAccessTokenAsync("invalidtoken", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthResponseDto?)null);
        // Act
        var result = await _authService.RefreshAccessTokenAsync("invalidtoken");
        // Assert
        result.Type.Should().Be(ServiceResultType.Unauthorized);
    }
    [Fact]
    public async Task RefreshAccessTokenAsync_ValidToken_ReturnsOkWithNewTokens()
    {
        // Arrange
        var response = new AuthResponseDto
        {
            Token = "access",
            RefreshToken = "refresh"
        };

        _jwtProviderMock
            .Setup(x => x.RefreshAccessTokenAsync("token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _authService.RefreshAccessTokenAsync("token");

        // Assert
        result.Type.Should().Be(ServiceResultType.Ok);
        result.Data.Should().BeEquivalentTo(response);
        #endregion
    }
}
