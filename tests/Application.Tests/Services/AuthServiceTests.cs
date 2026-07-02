using Application.Dto.DtoExtensions;
using Application.Dto.Jwt;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Providers.Contracts;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Enums;
using Xunit;

namespace Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly AuthService _authService;

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
            Options.Create(new JwtOptions()));
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "123"
        };

        _userManagerMock
            .Setup(userManager => userManager.FindByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

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
        var dto = new LoginRequestDto
        {
            Email = user.Email,
            Password = "wrongpass"
        };

        _userManagerMock
            .Setup(userManager => userManager.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(userManager => userManager.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(false);

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
        var dto = new LoginRequestDto
        {
            Email = user.Email,
            Password = "123"
        };
        var expectedResponse = new AuthResponseDto
        {
            Token = "access",
            RefreshToken = "refresh",
            User = user.ToAuthDto()
        };

        _userManagerMock
            .Setup(userManager => userManager.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(userManager => userManager.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);
        _jwtProviderMock
            .Setup(jwtProvider => jwtProvider.GenerateTokensAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.Ok);
        result.Data.Should().BeEquivalentTo(expectedResponse);
    }

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
            .Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.Conflict);
    }

    [Fact]
    public async Task RegisterAsync_Success_ReturnsNoContent()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "123456"
        };

        _userManagerMock
            .Setup(userManager => userManager.CreateAsync(It.IsAny<User>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Type.Should().Be(ServiceResultType.NoContent);
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        const string refreshToken = "invalidtoken";

        _jwtProviderMock
            .Setup(jwtProvider => jwtProvider.RefreshAccessTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthResponseDto?)null);

        // Act
        var result = await _authService.RefreshAccessTokenAsync(refreshToken);

        // Assert
        result.Type.Should().Be(ServiceResultType.Unauthorized);
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_ValidToken_ReturnsOkWithNewTokens()
    {
        // Arrange
        const string refreshToken = "token";
        var response = new AuthResponseDto
        {
            Token = "access",
            RefreshToken = "refresh"
        };

        _jwtProviderMock
            .Setup(jwtProvider => jwtProvider.RefreshAccessTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _authService.RefreshAccessTokenAsync(refreshToken);

        // Assert
        result.Type.Should().Be(ServiceResultType.Ok);
        result.Data.Should().BeEquivalentTo(response);
    }
}
