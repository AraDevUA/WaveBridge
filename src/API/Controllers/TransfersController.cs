using API.Extensions;
using Application.Dto.Requests.Transfers;
using Application.Services.Contracts;
using Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User,SuperAdmin")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;
    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet("sources/{source}/playlists")]
    public async Task<IResult> GetPlaylistsAsync([FromRoute] StreamingService source, [FromQuery] PlaylistPagedRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _transferService.GetPlaylistsAsync(User.GetUserId(), source, dto, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("sources/{source}/playlists/{playlistId}/tracks")]
    public async Task<IResult> GetPlaylistTracksAsync([FromRoute] StreamingService source, [FromRoute] string playlistId, CancellationToken cancellationToken)
    {
        var result = await _transferService.GetPlaylistTracksAsync(User.GetUserId(), source, playlistId, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("sources/{source}/liked-tracks")]
    public async Task<IResult> GetLikedTracksAsync([FromRoute] StreamingService source, [FromQuery] LikedTracksPagedRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _transferService.GetLikedTracksAsync(User.GetUserId(), source, dto, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{transferId:guid}")]
    public async Task<IResult> GetTransferAsync([FromRoute] Guid transferId, CancellationToken cancellationToken)
    {
        var result = await _transferService.GetTransferAsync(User.GetUserId(), transferId, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPost]
    public async Task<IResult> StartTransferAsync([FromBody] StartTransferRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _transferService.StartTransferAsync(User.GetUserId(), dto, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("history")]
    public async Task<IResult> GetTransferHistoryAsync([FromQuery] TransferHistoryRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _transferService.GetTransferHistoryAsync(User.GetUserId(), dto, cancellationToken);
        return result.ToApiResult();
    }
}
