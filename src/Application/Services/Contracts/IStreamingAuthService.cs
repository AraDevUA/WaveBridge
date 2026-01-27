using Application.Results.Interfaces;
using Domain.Enums;

namespace Application.Services.Contracts;

public interface IStreamingAuthService
{
    Task<IServiceResult> GetAuthorizationUrlAsync(StreamingService service, Guid userId);
    Task<IServiceResult> HandleCallbackAsync(string state, StreamingService service, string code);
}
