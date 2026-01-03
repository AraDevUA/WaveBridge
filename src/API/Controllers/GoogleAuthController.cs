using API.Extensions;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    public GoogleAuthController(IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }

    [HttpGet("redirect")]
    public async Task<IResult> RedirectOnOAuthServer(CancellationToken cancellationToken)
    {
        var redirectUrl = await _googleAuthService.RedirectOnOAuthServerAsync(cancellationToken);
        return Results.Redirect(redirectUrl);
    }

    [HttpGet("callback")]
    public async Task<IResult> OAuthCallback([FromQuery] string code, CancellationToken cancellationToken)
    {
        var result = await _googleAuthService.OAuthCallbackAsync(code, cancellationToken);
        return result.ToApiResult();
    }
}
