using Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IEntity<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public DateTimeOffset? CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public string? AvatarUrl { get; set; }
    public virtual ICollection<UserStreamingConnection> StreamingConnections { get; set; } = [];
    public virtual ICollection<UserOAuthConnection> OAuthConnections { get; set; } = [];
}

