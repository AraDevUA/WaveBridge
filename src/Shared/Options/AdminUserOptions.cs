using System.ComponentModel.DataAnnotations;

namespace Shared.Options;

public record AdminUserOptions
{
    [Required]
    public required string UserName { get; init; }
    [Required]
    public required string Email { get; init; }
    [Required]
    public required string Password { get; init; }
}
