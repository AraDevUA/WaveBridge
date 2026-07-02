namespace Application.Dto.Response.Auth;

public record AuthUserResponseDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
}
