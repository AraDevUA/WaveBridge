using Application.Dto.DtoExtensions;
using Application.Dto.Jwt;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Providers.Contracts;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;

    //TODO:delete, code for cookies
    private readonly JwtOptions _jwtOptions;
    private readonly IHttpContextAccessor _httpContext; 
    public AuthService(UserManager<User> userManager, IJwtProvider jwtProvider, IHttpContextAccessor httpContext, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        //TODO:delete, code for cookies
        _httpContext=httpContext;
        _jwtOptions=jwtOptions.Value;
    }
    public async Task<IServiceResult> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return ServiceResults.Unauthorized();

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return ServiceResults.Unauthorized();

        //return ServiceResults.Ok(await _jwtProvider.GenerateTokensAsync(user, cancellationToken));

        //TODO:delete, code for cookies
        var tokens = await _jwtProvider.GenerateTokensAsync(user, cancellationToken);
        SetTokensInCookies(tokens);
        return ServiceResults.Ok(tokens);
    }
    public async Task<IServiceResult> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = dto.ToEntity();
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            //TODO: improve error handling
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            return ServiceResults.BadRequest(errors);
        }

        return ServiceResults.Ok(user.ToDto());
    }
    public async Task<IServiceResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var result = await _jwtProvider.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        return result is null ? ServiceResults.Unauthorized() : ServiceResults.Ok(result);
    }
    //TODO:delete, code for cookies
    private void SetTokensInCookies(AuthResponseDto tokens)
    {
        if (_httpContext.HttpContext == null)
            return;

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.ExpiresHours)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenLifetimeDays)
        };

        _httpContext.HttpContext.Response.Cookies.Append("access_token", tokens.Token, accessCookieOptions);
        _httpContext.HttpContext.Response.Cookies.Append("refresh_token", tokens.RefreshToken, refreshCookieOptions);
    }
}