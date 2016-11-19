using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Devices.Permissions;
using SyncthingWeb.Helpers;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.Areas.Devices.Controllers
{
    [Area("Devices")]
    public class HomeController : ExtendedController
    {
        private readonly ISyncthingContextFactory syncthingContextFactory;

        public HomeController(ISyncthingContextFactory syncthingContextFactory)
        {
            this.syncthingContextFactory = syncthingContextFactory;
        }

        public async Task<ActionResult> Index()
        {
            if (!await this.Authorizer.AuthorizeAsync(DevicePermissions.View))
            {
                return new UnauthorizedResult();
            }

            var devices = (await this.syncthingContextFactory.GetContext()).Devices;

            return this.View(devices);
        }

        public async Task<ActionResult> Folders(string deviceId, int version)
        {
            if (!await this.Authorizer.AuthorizeAsync(DevicePermissions.View))
            {
                return new UnauthorizedResult();
            }

            
            var folders = (await this.syncthingContextFactory.GetContext()).GetFolders(deviceId);
            this.ViewBag.Device = (await this.syncthingContextFactory.GetContext()).GetDevice(deviceId);
            this.ViewBag.Version = version;
            return this.PartialView(folders);
        }
    }
}