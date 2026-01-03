namespace Application.Dto.Options;

public record EndpointsOptions
{
    public string UserInfoEndpoint { get; init; }
    public string OAuthServerEndpoint { get; init; }
    public string TokenEndpoint { get; init; }
}
