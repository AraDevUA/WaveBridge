using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Request.Auth;

public record LoginRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
