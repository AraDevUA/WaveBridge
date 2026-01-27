using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options.Auth.Spotify;

public record SpotifyAuthOptions
{
    [Required]
    public string ClientId { get; init; }
    [Required]
    public string ClientSecret { get; init; }
    [Required]
    public string RedirectUri { get; init; }
    [Required]
    public SpotifyEndpointsOptions Endpoints { get; init; }
}
