using SyncthingWeb.Permissions;

namespace SyncthingWeb.Areas.Share.Permissions
{
    internal class SharePermissions : PermissionProviderBase
    {
        public static Permission Allow = new Permission("allow-share", "Allow sharing")
                                             {
                                                 Description =
                                                     "Allow users to create share link",
                                                 Group = "Sharing"
                                             };
    }
}