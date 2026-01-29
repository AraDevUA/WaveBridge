using Application.Dto.Options.Auth.Google;
using Application.Dto.Responses.Auth;
using Application.Helpers;
using Application.Helpers.Contracts;
using Application.Strateges.Abstractions;
using Microsoft.Extensions.Options;

namespace Application.Strateges.Auth;

public class YouTubeMusicAuthStrategy : IStreamingAuthStrategy
{
    private readonly IHttpClientHelper _httpClientHelper;
    private readonly GoogleAuthOptions _options;

    public YouTubeMusicAuthStrategy(IOptions<GoogleAuthOptions> options, IHttpClientHelper httpClientHelper)
    {
        _options = options.Value;
        _httpClientHelper = httpClientHelper;
    }
    public async Task<StreamingServiceAuthTokenDto> ExchangeCodeAsync(string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", _options.YoutubeMusicCallbackUri },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        return await _httpClientHelper.SendPostFormRequestAsync<StreamingServiceAuthTokenDto>(
            _options.Endpoints.TokenEndpoint,
            tokenRequest,
            throwOnError: true
        );
    }

    public string GetAuthorizationUrl(string state)
    {
        var scope = _options.Scopes.YouTubeScope;
        var redirectUri = Uri.EscapeDataString(_options.YoutubeMusicCallbackUri);

        return $"{_options.Endpoints.OAuthServerEndpoint}" +
               $"?client_id={_options.ClientId}" +
               $"&response_type=code" +
               $"&redirect_uri={redirectUri}" +
               $"&state={Uri.EscapeDataString(state)}" +
               $"&scope={Uri.EscapeDataString(scope)}" +
               "&access_type=offline";
    }

    public async Task<StreamingServiceAuthTokenDto> RefreshAsync(string refreshToken)
    {
        var refreshRequest = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
        };

        return await _httpClientHelper.SendPostFormRequestAsync<StreamingServiceAuthTokenDto>(
            _options.Endpoints.TokenEndpoint,
            refreshRequest,
            throwOnError: true
        );
    }
}