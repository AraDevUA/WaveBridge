
using Domain.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole<Guid>, IEntity<Guid>, IAuditableEntity, ISoftDeletableEntity
{
    public DateTimeOffset? CreatedUtc { get; set; }
    public DateTimeOffset? ModifiedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedUtc { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];
}
