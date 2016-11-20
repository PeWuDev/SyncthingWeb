using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Share.Helpers;
using SyncthingWeb.Areas.Share.Permissions;
using SyncthingWeb.Areas.Share.Providers.Implementation;
using SyncthingWeb.Commands.Implementation.SharedEntries;

namespace SyncthingWeb.Areas.Share.Controllers
{
    [Area("Share")]
    public class PublicController : ShareControllerBase
    {
        public override async Task<ActionResult> Configure(string id, string path)
        {
            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow)) return new UnauthorizedResult();
            if (!await this.HasAccess(id)) return new UnauthorizedResult();

            var userClaims = this.User;
            var usrId = userClaims != null ? this.UserManager.GetUserId(userClaims) : null;

            var shareId = await
                this.CommandFactory.Create<WriteOrOverrideSharedEntryCommand>()
                    .Setup(id, usrId, path, PublicShareProvider.GlobalId, null).GetAsync();

            return this.RedirectToAction("ShareSummary", new { id = shareId});
        }

        public override async Task<ActionResult> ShareSummary(int id)
        {
            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow)) return new UnauthorizedResult();

            var sh =  await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(id).GetAsync();
            if (sh == null) return this.NotFound();
            if (!await this.HasAccess(sh.Folder.FolderId)) return new UnauthorizedResult();

            this.ViewBag.SharedEntry = sh;
            return this.View(id);
        }

        [AllowAnonymous]
        public override async Task<ActionResult> Index(int id)
        {
            var sh = await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(id).GetAsync();
            if (sh == null) return this.NotFound();
            if (sh.Provider != PublicShareProvider.GlobalId) return this.NotFound();

            if (!await this.EnsureNonRemoved(sh)) return this.NotFound();

            return await this.GetFileResultForLink(sh);
        }
    }
}
