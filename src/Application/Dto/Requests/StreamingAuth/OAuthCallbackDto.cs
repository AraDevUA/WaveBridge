namespace Application.Dto.Requests.StreamingAuth;

public record OAuthCallbackDto
{
    public string Code { get; init; }
    public string State { get; init; }
}
