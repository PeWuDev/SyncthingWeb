using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Helpers;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Folders.Helpers
{
    public class FolderControllerBase :ExtendedController
    {
        public UserManager<ApplicationUser> UserManager
            =>
            (UserManager<ApplicationUser>)
            this.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));

        public async Task<bool> HasAccess(string folderId)
        {
            var cmd = this.CommandFactory.Create<IsAllowedFolderCommand>();
            var currentUser = this.User == null ? null : this.UserManager.GetUserId(this.User);
            if (currentUser == null) return false;

            return await cmd.Setup(currentUser, folderId).GetAsync();
        }

        public Task<bool> Exists(string folderId)
        {
            return this.CommandFactory.Create<ExistsFolderCommand>().Setup(folderId).GetAsync();
        }

        public Task<AllowedFolder> GetAllowed(string id)
        {
            return
                this.CommandFactory.Create<GetAllowedFolderCommand>()
                    .Setup(this.UserManager.GetUserId(this.User), id)
                    .GetAsync();
        }
    }
}