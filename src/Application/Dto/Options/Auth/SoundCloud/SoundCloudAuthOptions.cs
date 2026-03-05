using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options.Auth.SoundCloud;

public record SoundCloudAuthOptions
{
    [Required]
    public string ClientId { get; init; }
    [Required]
    public string ClientSecret { get; init; }
    [Required]
    public string RedirectUri { get; init; }
    [Required]
    public string? Scopes { get; init; }
    [Required]
    public SoundCloudEndpointsOptions Endpoints { get; init; }
}
