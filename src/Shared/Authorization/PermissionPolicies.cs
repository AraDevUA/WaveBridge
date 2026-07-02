namespace Shared.Authorization;

public static class PermissionPolicies
{
    public const string Prefix = "Permission:";

    public static string Build(string permission) => $"{Prefix}{permission}";
}
