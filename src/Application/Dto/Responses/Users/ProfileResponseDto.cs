namespace Application.Dto.Response.Users;

public record ProfileResponseDto
{
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? AvatarUrl { get; init; }
}

