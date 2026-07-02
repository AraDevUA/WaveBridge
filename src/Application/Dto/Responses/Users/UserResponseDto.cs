namespace Application.Dto.Response.Users;

public record UserResponseDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public IEnumerable<string> Roles { get; init; } = [];
}
