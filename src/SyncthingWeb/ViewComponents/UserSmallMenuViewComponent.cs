using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;

namespace SyncthingWeb.ViewComponents
{
    public class UserSmallMenuViewComponent : ViewComponent
    {
        private readonly ICommandFactory commandFactory;
        private readonly UserManager<ApplicationUser> userManager;

        public UserSmallMenuViewComponent(ICommandFactory commandFactory, UserManager<ApplicationUser> userManager)
        {
            this.commandFactory = commandFactory;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = this.userManager.GetUserId(this.UserClaimsPrincipal);
            return this.View(new SmallMenuViewModel
            {
                IsSuperAdmin = (await this.commandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync()).AdminId == userId
            });
        }
    }

    public class SmallMenuViewModel
    {
        public bool IsSuperAdmin { get; set; }
    }
}
