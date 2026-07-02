using API.Tests.Infrastructure;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace API.Tests.Auth;

public class AuthIntegrationTests
{
    [Fact]
    public async Task AuthFlow_RegisterLoginRefresh_UsesHttpPipelineAndPersistsRefreshTokens()
    {
        // Arrange
        using var factory = new WaveBridgeApiFactory();
        await factory.ResetDatabaseAsync(TestContext.Current.CancellationToken);

        var client = factory.CreateClient();
        var registerRequest = new RegisterRequestDto
        {
            UserName = "integration_user",
            Email = "integration@example.com",
            Password = "password1"
        };

        // Act
        var registerResponse = await client.PostAsJsonAsync(
            "/Auth/register",
            registerRequest,
            TestContext.Current.CancellationToken);

        var loginResponse = await client.PostAsJsonAsync(
            "/Auth/login",
            new LoginRequestDto
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            },
            TestContext.Current.CancellationToken);

        var refreshCookie = GetRefreshTokenCookie(loginResponse);
        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/Auth/refresh-token");
        refreshRequest.Headers.Add(HeaderNames.Cookie, $"refreshToken={refreshCookie}");

        var refreshResponse = await client.SendAsync(
            refreshRequest,
            TestContext.Current.CancellationToken);

        // Assert
        registerResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<AuthClientResponseDto>(
            cancellationToken: TestContext.Current.CancellationToken);
        loginPayload.Should().NotBeNull();
        loginPayload!.Token.Should().NotBeNullOrWhiteSpace();
        loginPayload.User.Email.Should().Be(registerRequest.Email);
        loginPayload.User.UserName.Should().Be(registerRequest.UserName);

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshPayload = await refreshResponse.Content.ReadFromJsonAsync<AuthClientResponseDto>(
            cancellationToken: TestContext.Current.CancellationToken);
        refreshPayload.Should().NotBeNull();
        refreshPayload!.Token.Should().NotBeNullOrWhiteSpace();
        GetRefreshTokenCookie(refreshResponse).Should().NotBe(refreshCookie);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = await dbContext.Users.SingleAsync(
            x => x.Email == registerRequest.Email,
            TestContext.Current.CancellationToken);
        user.UserName.Should().Be(registerRequest.UserName);

        var refreshTokens = await dbContext.Set<RefreshToken>()
            .Where(x => x.UserId == user.Id)
            .ToListAsync(TestContext.Current.CancellationToken);
        refreshTokens.Should().HaveCount(2);
        refreshTokens.Should().ContainSingle(x => x.IsRevoked);
        refreshTokens.Should().ContainSingle(x => !x.IsRevoked);
    }

    private static string GetRefreshTokenCookie(HttpResponseMessage response)
    {
        response.Headers.TryGetValues(HeaderNames.SetCookie, out var setCookieHeaders)
            .Should()
            .BeTrue("auth endpoints should set the refresh token as an HTTP-only cookie");

        var refreshTokenCookie = SetCookieHeaderValue
            .ParseList(setCookieHeaders!.ToList())
            .Single(cookie => cookie.Name.Value == "refreshToken");

        refreshTokenCookie.HttpOnly.Should().BeTrue();
        refreshTokenCookie.Value.Value.Should().NotBeNullOrWhiteSpace();

        return refreshTokenCookie.Value.Value!;
    }
}
