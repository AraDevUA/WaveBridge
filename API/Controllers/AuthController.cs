using API.Extensions;
using Application.Dto.Request.Auth;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
        return result.ToApiResult();
    }
    //[HttpPost("logout")]
    //public async Task<IResult> LogoutAsync()
    //{
    //    //TODO: Implement logout functionality 
    //}
    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshTokenAsync([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        return result.ToApiResult();
    }
    //public async Task<IResult> ResetPassword()
    //{//TODO: Implement password reset functionality
    //}
}