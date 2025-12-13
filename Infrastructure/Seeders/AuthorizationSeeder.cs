using Domain.Entities;
using Infrastructure.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Infrastructure.Seeders;

public sealed class AuthorizationSeeder
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
        await SeedRolesAsync();
        await SeedPermissionsAsync();
        await SeedRolePermissionsAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleOption in _options.RolePermissions)
        {
            var existingRole = await _roleManager.FindByIdAsync(roleOption.Id.ToString());
            if (existingRole != null)
                continue;

            var role = new Role
            {
                Id = roleOption.Id,
                Name = roleOption.Name
            };

            await _roleManager.CreateAsync(role);
        }
    }

    private async Task SeedPermissionsAsync()
    {
        var permissions = _options.RolePermissions
            .SelectMany(r => r.Permissions)
            .Distinct();

        foreach (var name in permissions)
        {
            var exists = await _permissionRepository.All.AnyAsync(p => p.Name == name);
            if (exists) continue;

            await _permissionRepository.CreateAsync(new Permission
            {
                Name = name
            });
        }
    }

    private async Task SeedRolePermissionsAsync()
    {
        using var transaction = await _rolePermissionRepository.BeginTransactionAsync();

        foreach (var roleOption in _options.RolePermissions)
        {
            foreach (var permName in roleOption.Permissions)
            {
                var permission = await _permissionRepository.All
                    .SingleOrDefaultAsync(p => p.Name == permName);

                if (permission is null)
                    continue; 

                var exists = await _rolePermissionRepository.All
                    .AnyAsync(rp => rp.RoleId == roleOption.Id && rp.PermissionId == permission.Id);

                if (exists)
                    continue;

                await _rolePermissionRepository.AddAsync(new RolePermission
                {
                    RoleId = roleOption.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await _rolePermissionRepository.SaveChangesAsync();
        await transaction.CommitAsync();
    }

}
