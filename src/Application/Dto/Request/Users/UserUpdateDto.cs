namespace Application.Dto.Request.Users;

public record UserUpdateDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
