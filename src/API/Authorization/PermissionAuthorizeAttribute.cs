using Microsoft.AspNetCore.Authorization;
using Shared.Authorization;

namespace API.Authorization;

public sealed class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string permission)
    {
        Policy = PermissionPolicies.Build(permission);
    }
}
