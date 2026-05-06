using API.Extensions;
using Application.Dto.Jwt;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace API.Controllers;
[ApiController]
[Route("auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly JwtOptions _jwtOptions;
    private readonly FrontendOptions _frontendOptions;

    public GoogleAuthController(
        IGoogleAuthService googleAuthService,
        IOptions<JwtOptions> jwtOptions,
        IOptions<FrontendOptions> frontendOptions)
    {
        _googleAuthService = googleAuthService;
        _jwtOptions = jwtOptions.Value;
        _frontendOptions = frontendOptions.Value;
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
        return Response.ToAuthFrontendRedirectResult(result, _jwtOptions, _frontendOptions);
    }
}
