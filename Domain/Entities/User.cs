using Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public virtual ICollection<UserConnection> Connections { get; set; }
    User()
    {
        Connections = [];
    }
}

