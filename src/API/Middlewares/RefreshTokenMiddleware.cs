using Application.Dto.Jwt;
using Application.Helpers;
using Application.Providers.Contracts;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Middlewares;

public class RefreshTokenMiddleware
{
    private const string RefreshTokenCookieName = "refreshToken";
    private const string AccessTokenHeaderName = "Authorization";
    private const string RefreshedAccessTokenHeaderName = "X-Access-Token";

    private readonly RequestDelegate _next;

    public RefreshTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtProvider jwtProvider, IOptions<JwtOptions> options)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers[AccessTokenHeaderName].FirstOrDefault();
        var accessToken = authHeader?.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

        if (!string.IsNullOrWhiteSpace(accessToken) && JwtHelper.IsExpired(accessToken))
        {
            var refreshToken = context.Request.Cookies[RefreshTokenCookieName];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var newTokens = await jwtProvider.RefreshAccessTokenAsync(refreshToken);
                if (newTokens is not null)
                {
                    AppendRefreshTokenCookie(context, newTokens.RefreshToken, options.Value.RefreshTokenLifetimeDays);

                    context.Request.Headers[AccessTokenHeaderName] = $"Bearer {newTokens.Token}";
                    context.Response.Headers[RefreshedAccessTokenHeaderName] = newTokens.Token;
                    SetPrincipal(context, newTokens.Token);
                }
                else
                {
                    DeleteRefreshTokenCookie(context);
                }
            }
        }

        await _next(context);
    }

    private static bool ShouldSkip(PathString path)
    {
        return path.StartsWithSegments("/Auth/login", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/Auth/register", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/Auth/refresh-token", StringComparison.OrdinalIgnoreCase);
    }

    private static void AppendRefreshTokenCookie(HttpContext context, string refreshToken, int refreshTokenLifetimeDays)
    {
        context.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenLifetimeDays)
        });
    }

    private static void DeleteRefreshTokenCookie(HttpContext context)
    {
        context.Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }

    private static void SetPrincipal(HttpContext context, string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var claims = jwtToken.Claims.ToList();
        var identity = new ClaimsIdentity(claims, "Bearer");
        context.User = new ClaimsPrincipal(identity);
    }
}
