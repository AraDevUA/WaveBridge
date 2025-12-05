using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Results.Interfaces;
using Domain.Entities;

namespace Application.Services.Contracts;

public interface IAuthService
{
    Task<IServiceResult> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> OAuthCallbackAsync(string code, CancellationToken cancellationToken = default );
    Task<string> RedirectOnOAuthServerAsync(CancellationToken cancellationToken = default);
    Task<IServiceResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<GoogleTokenResponseDto> RefreshAccessTokenAsync(UserOAuthConnection connection, CancellationToken cancellationToken = default);
}
