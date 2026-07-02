namespace Application.Dto.Request.Users;

public record UserRequestDto : PagedRequest
{
    public string? UserName { get; init; }
    public string? Email { get; init; }
}
