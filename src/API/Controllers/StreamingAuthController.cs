using API.Extensions;
using Application.Dto.Requests.StreamingAuth;
using Application.Services.Contracts;
using Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]

public class StreamingAuthController : ControllerBase
{
    private readonly IStreamingAuthService _service;

    public StreamingAuthController(IStreamingAuthService service)
    {
        _service = service;
    }

    [HttpGet("{service}/url")]
    [Authorize]
    public async Task<IResult> GetAuthUrl(StreamingService service, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _service.GetAuthorizationUrlAsync(service, userId, cancellationToken);

        return result.ToApiResult();
    }

    [HttpGet("{service}/callback")]
    [AllowAnonymous]
    public async Task<IResult> Callback(StreamingService service, [FromQuery] OAuthCallbackDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.HandleCallbackAsync(dto.State, service, dto.Code, cancellationToken);

        return result.ToApiResult();
    }
}
