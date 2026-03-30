using Application.Dto.Request.Transfers;
using Application.Results.Interfaces;
using Shared.Enums;

namespace Application.Services.Contracts;

public interface ITransferService
{
    Task<IServiceResult> StartPlaylistTransferAsync(Guid userId, StartPlaylistTransferRequestDto dto);
}
