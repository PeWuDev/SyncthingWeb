using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Models;

namespace SyncthingWeb.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using SyncthingWeb.Helpers;

    [Authorize]
    public class ErrorController: ExtendedController
    {
        public IActionResult Syncthing(string message)
        {
            var wm = new SyncthingErrorViewModel
            {
                Message = message
            };

            return this.View(wm);
        }

        public IActionResult Index()
        {
            return this.View();
        }

    }
}
