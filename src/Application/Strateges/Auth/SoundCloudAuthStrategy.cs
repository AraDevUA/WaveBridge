using Application.Dto.Options.Auth.SoundCloud;
using Application.Dto.Responses.Auth;
using Application.Helpers.Contracts;
using Application.Strateges.Abstractions;
using Microsoft.Extensions.Options;

namespace Application.Strateges.Auth;

public class SoundCloudAuthStrategy : IStreamingAuthStrategy
{
    private readonly IHttpClientHelper _httpClientHelper;
    private readonly SoundCloudAuthOptions _options;

    public SoundCloudAuthStrategy(IOptions<SoundCloudAuthOptions> options, IHttpClientHelper httpClientHelper)
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
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret },
                { "redirect_uri", _options.RedirectUri },
                { "code", code }
            },
            throwOnError: true
        );
    }

    public string GetAuthorizationUrl(string state)
    {
        return
            _options.Endpoints.AuthorizationEndpoint +
            $"?client_id={_options.ClientId}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            "&response_type=code" +
            $"&state={Uri.EscapeDataString(state)}";
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
