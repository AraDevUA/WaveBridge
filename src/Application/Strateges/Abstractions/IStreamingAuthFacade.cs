using Shared.Enums;

namespace Application.Strateges.Abstractions;

public interface IStreamingAuthFacade
{
    string GetAuthorizationUrl(StreamingService service, Guid userId);
    Task HandleCallbackAsync(string state, StreamingService service, string code, CancellationToken cancellationToken = default);
}

