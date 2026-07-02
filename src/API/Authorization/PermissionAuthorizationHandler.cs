using Microsoft.AspNetCore.Authorization;
using Shared.Authorization;

namespace API.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        var hasPermission = context.User.Claims.Any(claim =>
            claim.Type == CustomClaimTypes.Permission
            && string.Equals(claim.Value, requirement.Permission, StringComparison.Ordinal));

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
