using Application.Dto.DtoExtensions;
using Application.Dto.Options;
using Application.Dto.Options.Auth.Google;
using Application.Dto.Responses.Auth;
using Application.Dto.Responses.Auth.Google;
using Application.Helpers;
using Application.Helpers.Contracts;
using Application.Localization;
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

public class GoogleAuthService : IGoogleAuthService
{
    private readonly GoogleAuthOptions _googleOptions;
    private readonly EncryptionOptions _encryptionOptions;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IHttpClientHelper _httpClientHelper;
    private readonly IRepository<UserOAuthConnection, Guid> _userOAuthConnectionRepository;
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;
    public GoogleAuthService(
            IOptions<GoogleAuthOptions> googleOptions,
            IOptions<EncryptionOptions> encryptionOptions,
            IHttpContextAccessor httpContext,
            IHttpClientHelper httpClientHelper,
            IRepository<UserOAuthConnection, Guid> userOAuthConnectionRepository,
            UserManager<User> userManager,
            IJwtProvider jwtProvider)
    {
        _googleOptions = googleOptions.Value;
        _encryptionOptions = encryptionOptions.Value;
        _httpContext = httpContext;
        _httpClientHelper = httpClientHelper;
        _userOAuthConnectionRepository = userOAuthConnectionRepository;
        _userManager=userManager;
        _jwtProvider=jwtProvider;
    }
    public async Task<UserOAuthConnection?> GetUserOAuthConnectionAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        return await _userOAuthConnectionRepository.FindAsync(cancellationToken, Id);
    }
    public async Task<string> RedirectOnOAuthServerAsync(CancellationToken cancellationToken = default)
    {
        var scope = _googleOptions.Scopes.UserInfoScope;
        var redirectUrl = _googleOptions.CallbackUri;

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
        var googleUser = await _httpClientHelper.SendGetRequestAsync<GoogleUserInfoResponseDto>(_googleOptions.Endpoints.UserInfoEndpoint, accessToken: tokenResult.AccessToken);
        
        var user = await _userManager.FindByEmailAsync(googleUser.Email);

        if (user is null)
        {
            user = googleUser.ToUserEntity();
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return ServiceResults.Failed(SystemMessages.InternalServerError);
        }

        await UpsertGoogleConnectionAsync(user, googleUser, tokenResult, cancellationToken);

        return ServiceResults.Ok(await _jwtProvider.GenerateTokensAsync(user, cancellationToken));
    }
    public async Task<GoogleTokenResponseDto> RefreshAccessTokenAsync(UserOAuthConnection connection, CancellationToken cancellationToken = default)
    {
        if (connection is null)
            throw new InvalidOperationException("Google connection not found.");

        var decryptedRefreshToken = AesGcmHelper.Decrypt(connection.RefreshToken, _encryptionOptions.KeyBase64);

        var refreshParams = new Dictionary<string, string>()
        {
            { "client_id", _googleOptions.ClientId },
            { "client_secret", _googleOptions.ClientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token", decryptedRefreshToken },
        };
        var tokenResponse = await _httpClientHelper.SendPostFormRequestAsync<GoogleTokenResponseDto>(_googleOptions.Endpoints.TokenEndpoint, refreshParams, cancellationToken: cancellationToken);

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
        var url = QueryHelpers.AddQueryString(_googleOptions.Endpoints.OAuthServerEndpoint, queryParams);
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

        var tokenResponse = await _httpClientHelper.SendPostFormRequestAsync<GoogleTokenResponseDto>(_googleOptions.Endpoints.TokenEndpoint, authParams, cancellationToken: cancellationToken);
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
