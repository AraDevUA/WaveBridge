using Application.Dto.DtoExtensions;
using Application.Dto.Request.Auth;
using Application.Providers.Contracts;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Application.Localization;
namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    public AuthService(UserManager<User> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }
    public async Task<IServiceResult> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return ServiceResults.Unauthorized();

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return ServiceResults.Unauthorized();

        return ServiceResults.Ok(await _jwtProvider.GenerateTokensAsync(user, cancellationToken));
    }
    public async Task<IServiceResult> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        var user = dto.ToEntity();
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(error => error.Description));
            return ServiceResults.Failed(string.IsNullOrWhiteSpace(errors) ? SystemMessages.InternalServerError : errors);
        }

        return ServiceResults.NoContent();
    }
    public async Task<IServiceResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var result = await _jwtProvider.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        return result is null ? ServiceResults.Unauthorized() : ServiceResults.Ok(result);
    }
}