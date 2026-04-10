using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Results.Interfaces;
using Shared.Enums;

namespace Application.Services.Contracts;

public interface ITransferService
{
    Task<IServiceResult> StartTransferAsync(Guid userId, StartTransferRequestDto dto);
    Task<IServiceResult> GetPlaylistsAsync(Guid userId, StreamingService source, PagedRequest pagedRequest);
    Task<IServiceResult> GetPlaylistTracksAsync(Guid userId, StreamingService source, string playlistId);
    Task<IServiceResult> GetLikedTracksAsync(Guid userId, StreamingService source, LikedTracksPagedRequestDto dto);
    Task<IServiceResult> GetTransferAsync(Guid userId, Guid transferId);
}
