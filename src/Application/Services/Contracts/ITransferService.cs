using Application.Dto;
using Application.Dto.Requests.Transfers;
using Application.Results.Interfaces;
using Shared.Enums;

namespace Application.Services.Contracts;

public interface ITransferService
{
    Task<IServiceResult> StartTransferAsync(Guid userId, StartTransferRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetPlaylistsAsync(Guid userId, StreamingService source, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetPlaylistTracksAsync(Guid userId, StreamingService source, string playlistId, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetLikedTracksAsync(Guid userId, StreamingService source, LikedTracksPagedRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetTransferAsync(Guid userId, Guid transferId, CancellationToken cancellationToken = default);
    Task<IServiceResult> GetTransferHistoryAsync(Guid userId, TransferHistoryRequestDto dto, CancellationToken cancellationToken = default);
}
