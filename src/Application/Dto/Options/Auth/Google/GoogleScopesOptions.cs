namespace Application.Dto.Options.Auth.Google;

public record GoogleScopesOptions
{
    public string UserInfoScope { get; init; }
    public string YouTubeScope { get; init; }
}
