namespace Application.Dto.Response.Auth;

public record AuthClientResponseDto
{
    public string Token { get; init; }
    public AuthUserResponseDto User { get; init; }
}
