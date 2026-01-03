using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options;

public record GoogleAuthOptions
{
    [Required]
    public string ClientId { get; init; }
    [Required]
    public string ClientSecret { get; init; }
    [Required]
    public string CallbackUri { get; init; }
    [Required]
    public ScopesOptions Scopes { get; init; }
    [Required]
    public EndpointsOptions Endpoints { get; init; }
}
