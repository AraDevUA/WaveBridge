using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Application.Strateges.Abstractions;
using Shared.Enums;

namespace Application.Services;

public class StreamingAuthService : IStreamingAuthService
{
    private readonly IStreamingAuthFacade _facade;

    public StreamingAuthService(IStreamingAuthFacade facade)
    {
        _facade = facade;
    }
    public async Task<IServiceResult> GetAuthorizationUrlAsync(StreamingService service, Guid userId, CancellationToken cancellationToken = default)
    {
        var result = _facade.GetAuthorizationUrl(service, userId);

        return ServiceResults.Ok(result);
    }

    public async Task<IServiceResult> HandleCallbackAsync(string state, StreamingService service, string code, CancellationToken cancellationToken = default)
    {
        await _facade.HandleCallbackAsync(state, service, code, cancellationToken);

        return ServiceResults.NoContent();
    }
}
