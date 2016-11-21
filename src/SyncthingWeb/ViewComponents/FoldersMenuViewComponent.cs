using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncthing.Integration;
using SyncthingWeb.Authorization;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Models;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.ViewComponents
{
    [Area("Folders")]
    public class FoldersMenuViewComponent : ViewComponent
    {
        private readonly ISyncthingContextFactory syncthingContextFactory;
        private readonly IAuthorizer authorizer;
        private readonly ICommandFactory commandFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoldersMenuViewComponent(ISyncthingContextFactory syncthingContextFactory, IAuthorizer authorizer, ICommandFactory commandFactory, UserManager<ApplicationUser> userManager)
        {
            this.syncthingContextFactory = syncthingContextFactory;
            this.authorizer = authorizer;
            this.commandFactory = commandFactory;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ctx = await this.syncthingContextFactory.GetContext();

            var isSuperAdmin =  await this.authorizer.IsSuperAdminAsync();;

            return
                this.View(new MenuViewComponentParam(ctx,
                    isSuperAdmin
                        ? null
                        : new HashSet<string>(
                            await
                                this.commandFactory.Create<GetAllowedFoldersCommand>()
                                    .Setup(this._userManager.GetUserId(this.UserClaimsPrincipal))
                                    .GetAsync().ContinueWith(task => task.Result.Keys))));
        }

        public class MenuViewComponentParam
        {
            public MenuViewComponentParam(SyncthingContext context, HashSet<string> allowedFolder)
            {
                Context = context;
                AllowedFolder = allowedFolder;
            }

            public SyncthingContext Context { get; }
            public HashSet<string> AllowedFolder { get; }
        }
    }
}
