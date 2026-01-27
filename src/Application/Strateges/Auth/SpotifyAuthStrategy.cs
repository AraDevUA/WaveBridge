using Application.Dto.Options.Auth.Spotify;
using Application.Dto.Responses.Auth;
using Application.Helpers;
using Application.Strateges.Abstractions;
using Microsoft.Extensions.Options;

namespace Application.Strateges.Auth;

public class SpotifyAuthStrategy : IStreamingAuthStrategy
{
    private readonly HttpClientHelper _httpClientHelper;
    private readonly SpotifyAuthOptions _options;
    public SpotifyAuthStrategy(IOptions<SpotifyAuthOptions> options, HttpClientHelper httpClientHelper)
    {
        _options = options.Value;
        _httpClientHelper = httpClientHelper;
    }

    public async Task<StreamingServiceAuthTokenDto> ExchangeCodeAsync(string code)
    {
        return await _httpClientHelper.SendPostFormRequestAsync<StreamingServiceAuthTokenDto>(
              _options.Endpoints.TokenEndpoint,
              new Dictionary<string, string>
              {
              { "grant_type", "authorization_code" },
              { "code", code },
              { "redirect_uri", _options.RedirectUri },
              { "client_id", _options.ClientId },
              { "client_secret", _options.ClientSecret }
              },
              throwOnError: true
         );
    }
    public string GetAuthorizationUrl(string state)
    {
        return
            _options.Endpoints.OAuthServerEndpoint +
            $"?client_id={_options.ClientId}" +
            "&response_type=code" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&state={Uri.EscapeDataString(state)}" +
            "&scope=user-read-private+user-library-read+user-follow-read+playlist-read-private+playlist-read-collaborative+playlist-modify-public+playlist-modify-private+user-library-modify+user-follow-modify";
    }

    public async Task<StreamingServiceAuthTokenDto> RefreshAsync(string refreshToken)
    {
        return await _httpClientHelper.SendPostFormRequestAsync<StreamingServiceAuthTokenDto>(
            _options.Endpoints.TokenEndpoint,
            new Dictionary<string, string>
            {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", _options.ClientId },
            { "client_secret", _options.ClientSecret }
            },
            throwOnError: true
        );
    }
}
