namespace Application.Dto.Request.Auth;

public record OAuthUserRequestDto
{
    public string Email { get; init; } = default!;
    public string? UserName { get; init; }
    public bool EmailConfirmed { get; init; } = true;
}
