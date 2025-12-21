    using Application.Dto.Jwt;
    using Application.Helpers;
    using Application.Providers.Contracts;
    using Microsoft.Extensions.Options;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    namespace API.Middlewares;
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RefreshTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IJwtProvider jwtProvider, IOptions<JwtOptions> options)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var accessToken = authHeader?.Split(" ").Last();

            if (!string.IsNullOrEmpty(accessToken) && JwtHelper.IsExpired(accessToken))
            {
                var refreshToken = context.Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var newTokens = await jwtProvider.RefreshAccessTokenAsync(refreshToken);
                    if (newTokens != null)
                    {
                        context.Response.Cookies.Append("refreshToken", newTokens.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTimeOffset.UtcNow.AddDays(options.Value.RefreshTokenLifetimeDays)
                        });

                        context.Request.Headers["Authorization"] = $"Bearer {newTokens.Token}";

                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(newTokens.Token);

                        var claims = jwtToken.Claims.ToList();
                        var identity = new ClaimsIdentity(claims, "Bearer");
                        context.User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        context.Response.Cookies.Delete("refreshToken");
                    }
                }
            }

            await _next(context);
        }
    }

