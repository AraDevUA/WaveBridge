namespace Application.Dto.Options.Auth.Spotify;

public record SpotifyEndpointsOptions
{
    public string OAuthServerEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
    public string UserProfileEndpoint { get; init; }
}