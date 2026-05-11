namespace Shared.Authorization;

public static class PermissionNames
{
    public static class Transfers
    {
        public const string Create = "Transfers.Create";
        public const string Read = "Transfers.Read";
    }

    public static class Users
    {
        public const string Read = "Users.Read";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
    }

    public static class Roles
    {
        public const string Manage = "Roles.Manage";
    }
}
