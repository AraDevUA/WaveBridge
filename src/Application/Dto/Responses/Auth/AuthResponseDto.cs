namespace Application.Dto.Response.Auth;

public record AuthResponseDto
{
    public string Token { get; init; }
    public string? RefreshToken { get; init; }
    public AuthUserResponseDto User { get; init; }
}
