using Application.Dto.Responses.Auth;
using Application.Results.Interfaces;
using Domain.Entities;

namespace Application.Services.Contracts;

public interface IGoogleAuthService
{
    Task<IServiceResult> OAuthCallbackAsync(string code, CancellationToken cancellationToken = default);
    Task<string> RedirectOnOAuthServerAsync(CancellationToken cancellationToken = default);
    Task<GoogleTokenResponseDto> RefreshAccessTokenAsync(UserOAuthConnection connection, CancellationToken cancellationToken = default);

}
