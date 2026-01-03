namespace Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; }
}
