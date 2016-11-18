using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Commands.Implementation.SharedEntries;
using SyncthingWeb.Helpers;
using SyncthingWeb.Models;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.Controllers
{
    public class HomeController : ExtendedController
    {
        private readonly ISyncthingContextFactory syncthingContextFactory;

        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ISyncthingContextFactory syncthingContextFactory, UserManager<ApplicationUser> userManager)
        {
            this.syncthingContextFactory = syncthingContextFactory;
            this.userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var ctx = await syncthingContextFactory.GetContext();
            var folders = ctx.Folders.AsEnumerable();


            if (!await this.Authorizer.IsSuperAdminAsync())
            {
                var allowedFolders = await
                    this.CommandFactory.Create<GetAllowedFoldersCommand>().Setup(this.userManager.GetUserId(this.User)).GetAsync();
                folders = folders.Where(fd => allowedFolders.ContainsKey(fd.Id));
            }


            var syncthingFolders = folders as SyncthingFolder[] ?? folders.ToArray();

            return View(new DashboardViewModel
            {
                FoldersId = syncthingFolders,
                Folders = syncthingFolders.Length,
                Shared =
                    await
                        this.CommandFactory.Create<GetSharedEntriesCountCommand>()
                            .Setup(this.userManager.GetUserId(this.User))
                            .GetAsync()
            });
        }

    }
}
