using Application.Dto.DtoExtensions;
using Application.Dto.Jwt;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Providers.Contracts;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;

namespace Application.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<UserManager<User>> _userManagerMock = null!;
    private Mock<IJwtProvider> _jwtProviderMock = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void SetUp()
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
    [Test]
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

    [Test]
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
    [Test]
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

    [Test]
    public async Task RegisterAsync_FailedCreate_ReturnsFailed()
    {
        // Arrange
        // Act
        // Assert
    }
    [Test]
    public async Task RegisterAsync_Success_ReturnsOkWithUserDto()
    {
        // Arrange
        // Act
        // Assert
    }
    #endregion
}
