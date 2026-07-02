using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Infrastructure.Seeders;

public class AuthorizationSeeder
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IRepository<Permission, Guid> _permissionRepository;
    private readonly IRepository<RolePermission, Guid> _rolePermissionRepository;
    private readonly AuthorizationOptions _options;

    public AuthorizationSeeder(
        RoleManager<Role> roleManager,
        IRepository<Permission, Guid> permissionRepository,
        IRepository<RolePermission, Guid> rolePermissionRepository,
        IOptions<AuthorizationOptions> options)
    {
        _roleManager = roleManager;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _options = options.Value;
    }

    public async Task SeedAsync()
    {
            using var transaction = await _rolePermissionRepository.BeginTransactionAsync();

        await SeedRolesAsync();
        await SeedPermissionsAsync();
        await SyncRolePermissionsAsync();

        await transaction.CommitAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleOption in _options.RolePermissions)
        {
            var existingRole = await _roleManager.FindByNameAsync(roleOption.Name);
            if (existingRole != null) continue;

            await _roleManager.CreateAsync(new Role { Name = roleOption.Name });
        }
    }

    private async Task SeedPermissionsAsync()
    {
        var permissions = _options.RolePermissions
            .SelectMany(r => r.Permissions)
            .Distinct();

        foreach (var name in permissions)
        {
            var exists = await _permissionRepository.All
                .AsNoTracking()
                .AnyAsync(p => p.Name == name);
            if (!exists)
            {
                await _permissionRepository.CreateAsync(new Permission { Name = name });
            }
        }
    }

    private async Task SyncRolePermissionsAsync()
    {
        var rolePermissionsInDb = await _rolePermissionRepository.All
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .ToListAsync();

        var allRolePerms = _options.RolePermissions
            .SelectMany(r => r.Permissions.Select(p => new { RoleName = r.Name, PermissionName = p }))
            .ToList();

        foreach (var rp in rolePermissionsInDb)
        {
            if (!allRolePerms.Any(x => x.RoleName == rp.Role.Name && x.PermissionName == rp.Permission.Name))
            {
                await _rolePermissionRepository.DeleteAsync(rp);
            }
        }

        foreach (var rolePerm in allRolePerms)
        {
            var role = await _roleManager.FindByNameAsync(rolePerm.RoleName);
            if (role is null) continue;

            var permission = await _permissionRepository.All
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Name == rolePerm.PermissionName);
            if (permission is null) continue;

            var exists = await _rolePermissionRepository.All
                .AsNoTracking()
                .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);

            if (!exists)
            {
                await _rolePermissionRepository.AddAsync(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await _rolePermissionRepository.SaveChangesAsync();
    }
}

