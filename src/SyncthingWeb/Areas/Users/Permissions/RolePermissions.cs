namespace SyncthingWebUI.Areas.Users.Permissions
{
    using System.Collections.Generic;
    using SyncthingWebUI.Permissions;

    internal class RolePermissions : PermissionProviderBase
    {
        public static Permission Assign = new Permission("assign-roles", "Assigning")
        {
            Description = "Allows to assign roles to user.",
            Group = "Roles",
            Implies = new HashSet<string>(new[] {"view-roles"})
        };

        public static Permission View = new Permission("view-roles", "Viewing")
        {
            Description = "Allows to view roles",
            Group = "Roles"
        };

        public static Permission Manage = new Permission("edit-roles", "Managing")
        {
            Description = "Allows manage roles.",
            Group = "Roles",
            Implies = new HashSet<string>(new[] { "view-roles", "assign-roles" })
        };
    }
}