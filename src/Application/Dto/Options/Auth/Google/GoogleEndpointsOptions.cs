namespace Application.Dto.Options.Auth.Google;

public record GoogleEndpointsOptions
{
    public string UserInfoEndpoint { get; init; }
    public string OAuthServerEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
}
