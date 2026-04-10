using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Infrastructure.Seeders;

public sealed class AdminUserSeeder
{
    private const string SuperAdminRoleName = "SuperAdmin";

    private readonly UserManager<User> _userManager;
    private readonly AdminUserOptions _options;

    public AdminUserSeeder(UserManager<User> userManager, IOptions<AdminUserOptions> options)
    {
        _userManager = userManager;
        _options = options.Value;
    }

    public async Task SeedAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.UserName) ||
            string.IsNullOrWhiteSpace(_options.Email) ||
            string.IsNullOrWhiteSpace(_options.Password))
        {
            return;
        }

        var user = await _userManager.FindByEmailAsync(_options.Email);
        if (user is null)
        {
            user = new User
            {
                UserName = _options.UserName,
                Email = _options.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, _options.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to seed admin user: {errors}");
            }
        }

        if (user.UserName != _options.UserName)
        {
            user.UserName = _options.UserName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to update admin user: {errors}");
            }
        }

        if (!await _userManager.IsInRoleAsync(user, SuperAdminRoleName))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, SuperAdminRoleName);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to assign SuperAdmin role: {errors}");
            }
        }
    }
}
