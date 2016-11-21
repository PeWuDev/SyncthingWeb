using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SyncthingWeb.Authorization;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;
using SyncthingWeb.Permissions;

namespace SyncthingWebUI.Authorization
{
    using System;
    using System.Threading.Tasks;

    public class DefaultAuthorizer : IAuthorizer
    {
        private readonly ICommandFactory commandFactory;
        private readonly IPermissionResolver permissionResolver;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        public DefaultAuthorizer(ICommandFactory commandFactory,  IPermissionResolver permissionResolver, IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager)
        {
            this.commandFactory = commandFactory;
            this.permissionResolver = permissionResolver;
            this.contextAccessor = contextAccessor;
            this.userManager = userManager;
        }

        public Task<bool> IsSuperAdminAsync(string userId = null)
        {
            if (userId == null)
            {
                var userClaims = this.contextAccessor.HttpContext.User;
                if (userClaims == null) return Task.FromResult(false);

                var curr = this.userManager.GetUserId(userClaims);

                if (curr == null) return Task.FromResult(false);

                userId = curr;
            }

            return this.commandFactory.Create<GetCurrentGeneralSettingsCommand>()
                .GetAsync()
                .ContinueWith(t => userId == t.Result.AdminId);
        }

        public async Task<bool> AuthorizeAsync(Permission permission, string userId = null)
        {
            if (permission == null) throw new InvalidOperationException();
            //return Task.FromResult(true);
            if (userId == null)
            {
                var userClaims = this.contextAccessor.HttpContext.User;
                if (userClaims == null) return false;

                var curr = this.userManager.GetUserId(userClaims);

                userId = curr;
            }

            if (await this.IsSuperAdminAsync(userId)) return true;

            return await this.permissionResolver.Authorize(permission, userId);
        }
    }
}