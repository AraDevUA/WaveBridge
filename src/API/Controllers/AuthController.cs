using API.Extensions;
using Application.Dto.Jwt;
using Application.Dto.Request.Auth;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<IResult> RegisterAsync([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPost("login")]
    public async Task<IResult> LoginAsync([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        return Response.ToAuthApiResult(result, _jwtOptions);
    }

    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Results.Unauthorized();

        var result = await _authService.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        return Response.ToAuthApiResult(result, _jwtOptions);
    }

    //public async Task<IResult> ResetPassword()
    //{//TODO: Implement password reset functionality
    //}
}
