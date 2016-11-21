using SyncthingWeb.Permissions;

namespace SyncthingWebUI.Areas.Users.Permissions
{
    using System.Collections.Generic;

    internal class UserPermissions : PermissionProviderBase
    {
        public static Permission View =
                new Permission("view-users", "View users")
                {
                    Description = "Allows to view users list",
                    Group = "Users"
                };

        public static Permission AllowedFolders = 
                new Permission("assign-folders-users", "Allowed folders")
                {
                    Description
                        =
                        "Set allowed folders to users",
                    Group =
                        "Users",
                    Implies = new HashSet<string>(new[] { "view-users" })
                };

        public static Permission Manage = new Permission("manage-users", "Manage users")
                                              {
                                                  Description =
                                                      "Allows to manage (create, delete etc) users",
                                                  Group = "Users",
                                                  Implies =
                                                      new HashSet<string>(
                                                      new[] { "view-users" })
                                              };
    }
}