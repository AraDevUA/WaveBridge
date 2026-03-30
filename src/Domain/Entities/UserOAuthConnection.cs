using Domain.Entities.Interfaces;
using Shared.Enums;

namespace Domain.Entities;

public class UserOAuthConnection : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ProviderUserId { get; set; } 
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; set; }
    public virtual User User { get; set; }
}
