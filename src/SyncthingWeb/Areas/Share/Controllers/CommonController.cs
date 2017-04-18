using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SyncthingWeb.Areas.Folders.Helpers;
using SyncthingWeb.Areas.Share.Models;
using SyncthingWeb.Areas.Share.Permissions;
using SyncthingWeb.Areas.Share.Providers;
using SyncthingWeb.Bus;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Commands.Implementation.SharedEntries;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Share.Controllers
{
    [Area("Share")]
    public class CommonController : FolderControllerBase
    {
        private readonly IEventBus eventBus;
        private readonly ILogger<CommonController> logger;
       

        public CommonController(IEventBus eventBus, ILogger<CommonController> logger)
        {
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public async Task<ActionResult> Share(string id, string path)
        {
            var usrClaims = this.User;
            var usrId = usrClaims == null ? null : this.UserManager.GetUserId(usrClaims);

            if (!await this.HasAccess(id))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot share \"{0}\" in folder \"{1}\" - user \"{2}\" has not permission to that folder",
                    path, id, usrId);
                return new UnauthorizedResult();
            }

            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot share \"{0}\" in folder \"{1}\" - user \"{2}\" has not permission to that folder",
                    path, id, usrId);
                return new UnauthorizedResult();
            }


            var allInstances = this.GetShares();
            return this.View(new CommonShareViewModel
            {
                FolderId = id,
                Path = path,
                Shares = allInstances
            });
        }

        public async Task<ActionResult> SharePreview(string id, string path)
        {
            var usrClaims = this.User;
            var usrId = usrClaims == null ? null : this.UserManager.GetUserId(usrClaims);

            if (!await this.HasAccess(id))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot share \"{0}\" in folder \"{1}\" - user \"{2}\" has no permission to that folder",
                    path, id, usrId);
                return new UnauthorizedResult();
            }

            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot share \"{0}\" in folder \"{1}\" - user \"{2}\" has no permission to that folder",
                    path, id, usrId);
                return new UnauthorizedResult();
            }

            var sh =
                (await
                    this.CommandFactory.Create<GetSharedEntriesForPathCommand>()
                        .Setup(id, usrId, path)
                        .GetAsync())
                    .FirstOrDefault().Value;

            if (sh == null) return this.NotFound();

            var settings = await this.CommandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync();

            var provider = this.GetShare(sh.Provider);


            return this.View(new SharePreviewViewModel
            {
                Entry = sh,
                Share = provider,
                RootUrl = settings.RootUrl
            });
        }


        public async Task<ActionResult> Stop(int id)
        {
            var sh = await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(id).GetAsync();
            if (sh == null) return this.NotFound();

            var usrClaims = this.User;
            var usrId = usrClaims == null ? null : this.UserManager.GetUserId(usrClaims);

            if (!await this.HasAccess(sh.Folder.FolderId))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot stop sharing {0}, user \"{1}\" has not access to folder {2}.", sh.Id, usrId,
                    sh.Folder.FolderId);
                return new UnauthorizedResult();
            }

            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow))
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot stop sharing {0}, user \"{1}\" has no permissions to share feature.", sh.Id,
                    usrId);
                return new UnauthorizedResult();
            }

            if (sh.OwnerId != usrId)
            {
                this.logger.LogError(ShareLoggingEvents.SharePermission,
                    "Cannot stop sharing {0} because user {1} is not owner of this entry.", sh.Id, usrId);
                return new UnauthorizedResult();
            }

            var fdid = sh.Folder.FolderId;
            var path = sh.Path;

            await this.CommandFactory.Create<StopSharingCommand>().Setup(id).ExecuteAsync();

            if (!path.Contains(Path.DirectorySeparatorChar))
            {
                path = "";
            }
            else
            {
                var lastIdx = path.LastIndexOf(Path.DirectorySeparatorChar);
                path = path.Substring(0, lastIdx);
            }

            return this.RedirectToAction("Index", "Home", new {area = "Folders", id = fdid, path});
        }

        private IEnumerable<IShare> GetShares()
        {
            var coll = new ShareCollector();
            this.eventBus.Trigger<IShareCollector>(coll);

            return coll.Shares;
        }

        private IShare GetShare(Guid id)
        {
            return this.GetShares().SingleOrDefault(sh => sh.Id == id);
        }

        public class SharePreviewViewModel
        {
            public IShare Share { get; set; }
            public SharedEntry Entry { get; set; }

            public string RootUrl { get; set; }


            public bool BuildAbsoluteUrl(string partUrl, out string result)
            {
                Uri rootUri;
                if (!Uri.TryCreate(RootUrl, UriKind.Absolute, out rootUri))
                {
                    result = partUrl;
                    return false;
                }

                var mainUrl = rootUri.GetComponents(
                    UriComponents.Scheme | UriComponents.Host | UriComponents.SchemeAndServer,
                    UriFormat.Unescaped);

                result = mainUrl + (partUrl.StartsWith("/") ? "" : "/") + partUrl;
                return true;
            }
        }

    }
}