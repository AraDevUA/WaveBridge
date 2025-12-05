using Application.Dto.DtoExtensions;
using Application.Dto.Options;
using Application.Dto.Request.Auth;
using Application.Dto.Response.Auth;
using Application.Helpers;
using Application.Providers.Contracts;
using Application.Results;
using Application.Results.Interfaces;
using Application.Services.Contracts;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly GoogleAuthOptions _googleOptions;
    private readonly EncryptionOptions _encryptionOptions;
    private readonly IHttpContextAccessor _httpContext;
    private readonly HttpClientHelper _httpClientHelper;
    private readonly IRepository<UserOAuthConnection, Guid> _userOAuthConnectionRepository;

    public AuthService(UserManager<User> userManager, IJwtProvider jwtProvider, IOptions<GoogleAuthOptions> googleOptions, IOptions<EncryptionOptions> encryptionOptions, IHttpContextAccessor httpContext, HttpClientHelper httpClientHelper, IRepository<UserOAuthConnection, Guid> userOAuthConnectionRepository)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _googleOptions = googleOptions.Value;
        _encryptionOptions = encryptionOptions.Value;
        _httpContext = httpContext;
        _httpClientHelper = httpClientHelper;
        _userOAuthConnectionRepository = userOAuthConnectionRepository;
    }
    public async Task<UserOAuthConnection?> GetUserOAuthConnectionAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        var connection = await _userOAuthConnectionRepository.FindAsync(cancellationToken, Id);
        return connection;
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
            //TODO: improve error handling
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            return ServiceResults.BadRequest(errors);
        }

        return ServiceResults.Ok(user.ToDto());
    }
    public async Task<string> RedirectOnOAuthServerAsync(CancellationToken cancellationToken = default)
    {
        var scope = "openid profile email";
        var redirectUrl = "https://localhost:7270/Auth/callback";

        var codeVerifier = Guid.NewGuid().ToString();
        _httpContext.HttpContext.Session.SetString("codeVerifier", codeVerifier);
        var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

        var url = GenerateOAuthRequestUrl(scope, redirectUrl, codeChallenge, cancellationToken);
        return url;
    }
    public async Task<IServiceResult> OAuthCallbackAsync(string code, CancellationToken cancellationToken = default)
    {
        var codeVerifier = _httpContext.HttpContext.Session.GetString("codeVerifier");

        var tokenResult = await ExchangeCodeOnToken(code, codeVerifier, cancellationToken);
        // TODO: to options
        var googleUser = await _httpClientHelper.SendGetRequestAsync<GoogleUserInfoResponseDto>("https://www.googleapis.com/oauth2/v2/userinfo", accessToken: tokenResult.AccessToken);

        var user = await _userManager.FindByEmailAsync(googleUser.Email);

        if (user is null)
        {
            user = googleUser.ToUserEntity();
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return ServiceResults.Failed("Failed to create user.");
        }

        await UpsertGoogleConnectionAsync(user, googleUser, tokenResult, cancellationToken);

        return ServiceResults.Ok(await _jwtProvider.GenerateTokensAsync(user, cancellationToken));
    }
    public async Task<IServiceResult> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var result = await _jwtProvider.RefreshAccessTokenAsync(refreshToken, cancellationToken);
        return result is null ? ServiceResults.Unauthorized() : ServiceResults.Ok(result);
    }
    public async Task<GoogleTokenResponseDto> RefreshAccessTokenAsync(UserOAuthConnection connection, CancellationToken cancellationToken = default)
    {
        if (connection is null)
            throw new InvalidOperationException("Google connection not found.");

        //TODO: incapulate decryption
        var decryptedRefreshToken = AesGcmHelper.Decrypt(connection.RefreshToken, _encryptionOptions.KeyBase64);

        var refreshParams = new Dictionary<string, string>()
        {
            { "client_id", _googleOptions.ClientId },
            { "client_secret", _googleOptions.ClientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", decryptedRefreshToken },
        };
        var tokenResponse = await _httpClientHelper.SendPostFormRequestAsync<GoogleTokenResponseDto>(_googleOptions.TokenEndpoint, refreshParams, cancellationToken: cancellationToken);

        connection.AccessToken = tokenResponse.AccessToken;
        connection.AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        await _userOAuthConnectionRepository.UpdateAsync(connection, cancellationToken);
        return tokenResponse;
    }
    private string GenerateOAuthRequestUrl(string scope, string redirectUrl, string codeChallenge, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "client_id", _googleOptions.ClientId },
            { "redirect_uri", redirectUrl },
            { "response_type", "code" },
            { "scope", scope },
            { "code_challenge", codeChallenge },
            { "code_challenge_method", "S256" },
            { "access_type", "offline" },
        };
        var url = QueryHelpers.AddQueryString(_googleOptions.OAuthServerEndpoint, queryParams);
        return url;
    }
    private async Task<GoogleTokenResponseDto> ExchangeCodeOnToken(string code, string codeVerifier, CancellationToken cancellationToken = default)
    {
        var authParams = new Dictionary<string, string>
        {
            { "client_id", _googleOptions.ClientId },
            { "client_secret", _googleOptions.ClientSecret },
            { "code", code },
            { "code_verifier", codeVerifier },
            { "grant_type", "authorization_code" },
            { "redirect_uri", _googleOptions.CallbackUri }
        };

        var tokenResponse = await _httpClientHelper.SendPostFormRequestAsync<GoogleTokenResponseDto>(_googleOptions.TokenEndpoint, authParams, cancellationToken: cancellationToken);
        return tokenResponse;
    }
    private async Task UpsertGoogleConnectionAsync(User user, GoogleUserInfoResponseDto googleUser, GoogleTokenResponseDto tokenResult, CancellationToken cancellationToken = default)
    {
        var connection = await _userOAuthConnectionRepository.All
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (connection is null)
        {
            var newConnection = googleUser.ToEntity(tokenResult, user.Id);
            newConnection.RefreshToken = EncryptRefreshToken(tokenResult.RefreshToken);
            await _userOAuthConnectionRepository.CreateAsync(newConnection, cancellationToken);
        }
        else
        {
            connection.AccessToken = tokenResult.AccessToken;
            connection.AccessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(tokenResult.ExpiresIn);

            if (!string.IsNullOrEmpty(tokenResult.RefreshToken))
                connection.RefreshToken = EncryptRefreshToken(tokenResult.RefreshToken);

            await _userOAuthConnectionRepository.UpdateAsync(connection, cancellationToken);
        }
    }
    private string? EncryptRefreshToken(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        return AesGcmHelper.Encrypt(refreshToken, _encryptionOptions.KeyBase64);
    }
}