using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Request.Auth;

public record RegisterRequestDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
