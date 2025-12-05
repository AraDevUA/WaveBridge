using Application.Dto.Response.Auth;
using Application.Results.Interfaces;
using Domain.Entities;

namespace Application.Providers.Contracts;

public interface IJwtProvider
{
    Task<AuthResponseDto> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);
    Task<AuthResponseDto?> RefreshAccessTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default);
}
