using System.ComponentModel.DataAnnotations;

namespace Shared.Options;

public record AdminUserOptions
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; } 
}
