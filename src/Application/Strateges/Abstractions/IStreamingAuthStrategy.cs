using Application.Dto.Responses.Auth;

namespace Application.Strateges.Abstractions;

public interface IStreamingAuthStrategy
{
    string GetAuthorizationUrl(string state);
    Task<StreamingServiceAuthTokenDto> ExchangeCodeAsync(string code);
    Task<StreamingServiceAuthTokenDto> RefreshAsync(string refreshToken);
}