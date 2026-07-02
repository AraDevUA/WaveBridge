using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options.Auth.Google;

public record GoogleAuthOptions
{
    [Required]
    public string ClientId { get; init; }
    [Required]
    public string ClientSecret { get; init; }
    [Required]
    public string CallbackUri { get; init; }
    [Required]
    public string YoutubeMusicCallbackUri { get; init; }
    [Required]
    public GoogleScopesOptions Scopes { get; init; }
    [Required]
    public GoogleEndpointsOptions Endpoints { get; init; }
}
