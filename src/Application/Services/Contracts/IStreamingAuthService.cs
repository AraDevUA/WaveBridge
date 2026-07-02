using Application.Results.Interfaces;
using Shared.Enums;

namespace Application.Services.Contracts;

public interface IStreamingAuthService
{
    Task<IServiceResult> GetAuthorizationUrlAsync(StreamingService service, Guid userId, CancellationToken cancellationToken = default);
    Task<IServiceResult> HandleCallbackAsync(string state, StreamingService service, string code, CancellationToken cancellationToken = default);
}
