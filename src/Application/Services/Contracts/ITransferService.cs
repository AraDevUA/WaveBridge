using Application.Dto.Request.Transfers;
using Application.Results.Interfaces;

namespace Application.Services.Contracts;

public interface ITransferService
{
    Task<IServiceResult> StartPlaylistTransferAsync(Guid userId, StartPlaylistTransferRequestDto dto);
}
