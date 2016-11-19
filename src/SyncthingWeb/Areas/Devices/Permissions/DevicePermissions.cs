using SyncthingWebUI.Permissions;

namespace SyncthingWeb.Areas.Devices.Permissions
{
    internal class DevicePermissions : PermissionProviderBase
    {
        public static Permission View = new Permission("view-devices", "View devices")
                                            {
                                                Group = "Devices",
                                                Description =
                                                    "Allows to view device list"
                                            };


    }
}