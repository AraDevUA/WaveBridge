using Application.Dto.Response.Users;

namespace Application.Dto.Response.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string? RefreshToken { get; set; }
    public UserResponseDto User { get; set; }
}
