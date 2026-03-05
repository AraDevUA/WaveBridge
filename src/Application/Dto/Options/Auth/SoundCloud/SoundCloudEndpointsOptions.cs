using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options.Auth.SoundCloud;

public record SoundCloudEndpointsOptions
{
    [Required]
    public string AuthorizationEndpoint { get; init; }
    [Required]
    public string TokenEndpoint { get; init; }
    [Required]
    public string UserProfileEndpoint { get; init; }
}
