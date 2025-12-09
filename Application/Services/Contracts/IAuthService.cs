using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Results.Interfaces;
using Domain.Entities;

namespace Application.Services.Contracts;

public interface IAuthService
{
    Task<IServiceResult> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default);
    Task<IServiceResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
