namespace Shared.Options;
public class RolePermissionsOptions
{
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string[] Permissions { get; set; } = [];
}
