using Application.Dto.Jwt;
using Application.Dto.Response.Auth;
using Application.Providers.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace API.Middlewares;
//TODO: Refactoring after frontend
public class JwtValidationMiddleware
{
    //private readonly RequestDelegate _next;
    //private readonly JwtOptions _jwtOptions;
    //private readonly ILogger<JwtValidationMiddleware> _logger;

    //public JwtValidationMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions, ILogger<JwtValidationMiddleware> logger)
    //{
    //    _next = next;
    //    _jwtOptions = jwtOptions.Value;
    //    _logger = logger;
    //}

    //public async Task InvokeAsync(HttpContext context, IJwtProvider jwtProvider)
    //{
    //    var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

    //    if (authResult.Succeeded)
    //    {
    //        context.User = authResult.Principal;
    //        await _next(context);
    //        return;
    //    }

    //    if (!context.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
    //    {
    //        await _next(context);
    //        return;
    //    }

    //    AuthResponseDto? newTokens = null;

    //    try
    //    {
    //        newTokens = await jwtProvider.RefreshAccessTokenAsync(refreshToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Failed to refresh access token");
    //    }

    //    if (newTokens != null)
    //    {
    //        context.Response.Cookies.Append("access_token", newTokens.Token, CreateAccessCookieOptions());
    //        context.Response.Cookies.Append("refresh_token", newTokens.RefreshToken, CreateRefreshCookieOptions());

    //        var reauth = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
    //        if (reauth.Succeeded)
    //            context.User = reauth.Principal;
    //    }

    //    await _next(context);
    //}

    //private CookieOptions CreateAccessCookieOptions()
    //{
    //    return new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Secure = true,
    //        SameSite = SameSiteMode.Strict,
    //        Path = "/",
    //        Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.ExpiresHours)
    //    };
    //}

    //private CookieOptions CreateRefreshCookieOptions()
    //{
    //    return new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Secure = true,
    //        SameSite = SameSiteMode.Strict,
    //        Path = "/",
    //        Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays)
    //    };
    //}
}

