namespace Application.Dto.Request.Auth;

public class OAuthUserRequestDto
{
    public string Email { get; set; } = default!;
    public string? UserName { get; set; }
    public bool EmailConfirmed { get; set; } = true;
}
