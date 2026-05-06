using Application.Dto.Jwt;
using Application.Dto.Response.Auth;
using Application.Results.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Options;

namespace API.Extensions;

public static class AuthResultExtensions
{
    private const string RefreshTokenCookieName = "refreshToken";

    public static IResult ToAuthApiResult(
        this HttpResponse response,
        IServiceResult result,
        JwtOptions jwtOptions)
    {
        if (!result.IsSuccess || result.Data is not AuthResponseDto authResponse)
            return result.ToApiResult();

        AppendRefreshTokenCookieIfPresent(response, authResponse, jwtOptions);

        return Results.Ok(new AuthClientResponseDto
        {
            Token = authResponse.Token,
            User = authResponse.User
        });
    }

    public static IResult ToAuthFrontendRedirectResult(
        this HttpResponse response,
        IServiceResult result,
        JwtOptions jwtOptions,
        FrontendOptions frontendOptions)
    {
        if (!result.IsSuccess || result.Data is not AuthResponseDto authResponse)
            return result.ToApiResult();

        AppendRefreshTokenCookieIfPresent(response, authResponse, jwtOptions);

        var baseUri = (frontendOptions.BaseUri ?? string.Empty).TrimEnd('/');
        var callbackUri = $"{baseUri}/auth/callback";
        return Results.Redirect(callbackUri);
    }

    private static void AppendRefreshTokenCookieIfPresent(
        HttpResponse response,
        AuthResponseDto authResponse,
        JwtOptions jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(authResponse.RefreshToken))
            return;

        response.Cookies.Append(RefreshTokenCookieName, authResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.RefreshTokenLifetimeDays)
        });
    }
}
