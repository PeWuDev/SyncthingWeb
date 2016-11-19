using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Users.Models;
using SyncthingWeb.Caching;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Commands.Implementation.Users;
using SyncthingWeb.Helpers;
using SyncthingWeb.Syncthing;
using SyncthingWebUI.Areas.Users.Permissions;

namespace SyncthingWeb.Areas.Users.Controllers
{
    [Authorize]
    public class HomeController : ExtendedController
    {
        private readonly ISyncthingContextFactory syncthingContextFactory;
        private readonly ICache cache;

        public HomeController(ISyncthingContextFactory syncthingContextFactory, ICache cache)
        {
            this.syncthingContextFactory = syncthingContextFactory;
            this.cache = cache;
        }

        public async Task<ActionResult> Index(GetUsersQuery query)
        {
            if (!await this.Authorizer.AuthorizeAsync(UserPermissions.View))
            {
                return new UnauthorizedResult();
            }

            this.ViewBag.SuperAdmin =
                (await this.CommandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync()).AdminId;

            var vm = await this.CommandFactory.Create<PageUsersCommand>().Setup(query).AsPagingResultAsync();

            return this.View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Deactive(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(UserPermissions.Manage))
            {
                return new UnauthorizedResult();
            }

            if (await this.Authorizer.IsSuperAdminAsync(id))
            {
                this.Notifications.NotifyError("User is super admin - cannot deactive him.");
                return this.RedirectToAction("Index");
            }

            try
            {
                await
                    this.CommandFactory.Create<UpdateUserCommand>()
                        .Setup(id)
                        .Setup(u => u.IsEnabled = false)
                        .ExecuteAsync();
                this.Notifications.NotifyError("User locked successfully.");
            }
            finally
            {
                this.cache.Signal("is-user-lockout-" + id);
            }

            return this.RedirectToAction("Index");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Active(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(UserPermissions.Manage))
            {
                return new UnauthorizedResult();
            }

            try
            {
                await
                    this.CommandFactory.Create<UpdateUserCommand>()
                        .Setup(id)
                        .Setup(u => u.IsEnabled = true)
                        .ExecuteAsync();
                this.Notifications.NotifyError("User unlocked successfully.");
            }
            finally
            {
                this.cache.Signal("is-user-lockout-" + id);
            }

            return this.RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Allowed(string user, AllowedFolderForUserViewModel[] fd)
        {
            if (!await this.Authorizer.AuthorizeAsync(UserPermissions.AllowedFolders))
            {
                return new UnauthorizedResult();
            }

            if (!await this.CommandFactory.Create<ExistsUserCommand>().Setup(user).GetAsync())
                return this.NotFound();

            if (await this.Authorizer.IsSuperAdminAsync(user))
            {
                this.Notifications.NotifyError("User is super admin - cannot change allowed folders.");
                return this.RedirectToAction("Index");
            }

            try
            {
                await
                    this.CommandFactory.Create<AssignAllowedCommand>()
                        .Setup(user,
                            fd.Select(f => new AssignAllowedCommand.AllowedParam(f.FolderId))
                                .ToArray())
                        .ExecuteAsync();
            }
            catch (Exception ex)
            {
                this.Logger.ErrorException(ex, "Error while assigning folders to user {0}", user);
                this.Notifications.NotifyError("Error while saving allowed folders.");
                return this.RedirectToAction("Allowed", new { id = user });
            }

            this.Notifications.NotifySuccess("Allowed folders saved successfully");
            return this.RedirectToAction("Index");
        }

        public async Task<ActionResult> Allowed(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(UserPermissions.AllowedFolders))
            {
                return new UnauthorizedResult();
            }

            if (await this.Authorizer.IsSuperAdminAsync(id))
            {
                this.Notifications.NotifyError("User is super admin - cannot change allowed folders.");
                return this.RedirectToAction("Index");
            }

            var usr = await this.CommandFactory.Create<GetUserCommand>().Include("Allowed").Setup(id).GetAsync();
            var folders = (await this.syncthingContextFactory.GetContext()).Folders;

            this.ViewBag.Folders = folders;
            return this.View(usr);
        }
    }
}