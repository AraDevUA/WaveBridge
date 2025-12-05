using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Options;

public record GoogleAuthOptions
{
    [Required]
    public string ClientId { get; init; }
    [Required]
    public string ClientSecret { get; init; }
    //TODO: delete if not used
    [Required]
    public string RedirectUri { get; init; }
    [Required]
    public string CallbackUri { get; init; }
    [Required]
    public string OAuthServerEndpoint { get; init; }
    [Required]
    public string TokenEndpoint { get; init; }
}
