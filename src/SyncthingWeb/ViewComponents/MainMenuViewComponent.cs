using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Devices.Permissions;
using SyncthingWeb.Authorization;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;
using SyncthingWebUI.Areas.Users.Permissions;

namespace SyncthingWeb.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly ICommandFactory commandFactory;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthorizer authorizer;

        public MainMenuViewComponent(ICommandFactory commandFactory, UserManager<ApplicationUser> userManager, IAuthorizer authorizer)
        {
            this.commandFactory = commandFactory;
            this.userManager = userManager;
            this.authorizer = authorizer;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = this.userManager.GetUserId(this.UserClaimsPrincipal);

            var model = new MainMenuModel
            {
                IsSuperAdmin =
                    (await this.commandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync()).AdminId == userId,
                CanUserView = await this.authorizer.AuthorizeAsync(UserPermissions.View),
                CanRoleView = await this.authorizer.AuthorizeAsync(RolePermissions.View),
                CanDeviceView = await this.authorizer.AuthorizeAsync(DevicePermissions.View)
            };

            return this.View(model);
        }
    }


    public class MainMenuModel
    {
        public bool CanUserView { get; set; }
        public bool CanRoleView { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool CanDeviceView { get; set; }
    }
}
