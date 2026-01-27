using API.Extensions;
using Application.Dto.Requests.StreamingAuth;
using Application.Services.Contracts;
using Domain.Enums;
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

    public async Task<IResult> GetAuthUrl(StreamingService service)
    {
        var userId = User.GetUserId();
        var result = await _service.GetAuthorizationUrlAsync(service, userId);

        return result.ToApiResult();
    }

    [HttpGet("{service}/callback")]
    [AllowAnonymous]
    public async Task<IResult> Callback(StreamingService service, [FromQuery] OAuthCallbackDto dto)
    {
        var result = await _service.HandleCallbackAsync(dto.State, service, dto.Code);

        return result.ToApiResult();
    }
}
