using Application.Dto.DtoExtensions;
using Application.Dto.Jwt;
using Application.Dto.Response.Auth;
using Application.Helpers;
using Application.Providers.Contracts;
using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly IRepository<RefreshToken, Guid> _refreshTokenRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<JwtProvider> _logger;
    public JwtProvider(IOptions<JwtOptions> options, IRepository<RefreshToken, Guid> refreshTokenRepository, UserManager<User> userManager, ILogger<JwtProvider> logger)
    {
        _options = options.Value;
        _refreshTokenRepository = refreshTokenRepository;
        _userManager = userManager;
        _logger = logger;
    }
    public async Task<AuthResponseDto> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);

        var rawToken = Guid.NewGuid().ToString();
        var hashedToken = Sha256Helper.ComputeHash(rawToken);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays),
            IsRevoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

        return new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = rawToken,
            User = user.ToDto(roles)
        };
    }

    public async Task<AuthResponseDto?> RefreshAccessTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var hashedValue = Sha256Helper.ComputeHash(refreshTokenValue);

        var refreshToken = await _refreshTokenRepository.All
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == hashedValue, cancellationToken);

        if (refreshToken is null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        if (refreshToken.IsRevoked)
        {
            await RevokeAllUserRefreshTokensAsync(refreshToken.UserId, cancellationToken);
            return null;
        }

        var user = refreshToken.User;
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = GenerateAccessToken(user, roles);

        refreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        var newRaw = Guid.NewGuid().ToString("N");
        var newHashed = Sha256Helper.ComputeHash(newRaw);

        var newRefresh = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newHashed,
            ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays),
            IsRevoked = false
        };

        await _refreshTokenRepository.CreateAsync(newRefresh, cancellationToken);

        return new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRaw, 
            User = user.ToDto(roles)
        }; 
    }
    private string GenerateAccessToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tokens = await _refreshTokenRepository.All
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _refreshTokenRepository.SaveChangesAsync();

        _logger.LogInformation("All refresh tokens revoked for user {UserId}", userId);
    }
}
