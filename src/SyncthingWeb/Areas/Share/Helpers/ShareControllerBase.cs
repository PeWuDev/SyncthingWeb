using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SyncthingWeb.Areas.Folders.Helpers;
using SyncthingWeb.Areas.Folders.Services;
using SyncthingWeb.Commands.Implementation.SharedEntries;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Share.Helpers
{
    public abstract class ShareControllerBase : FolderControllerBase
    {
        private readonly ILogger<ShareControllerBase> logger;

        protected ShareControllerBase(ILogger<ShareControllerBase> logger)
        {
            this.logger = logger;
        }

        public ISyncthingFileFetcher FileFetcher
            => (ISyncthingFileFetcher) this.HttpContext.RequestServices.GetService(typeof(ISyncthingFileFetcher));

        public abstract Task<ActionResult> Configure(string id, string path);
        public abstract Task<ActionResult> ShareSummary(int id);
        public abstract Task<ActionResult> Index(int id);

        public async Task<FileResult> GetFileResultForLink(SharedEntry entry)
        {
            var di = await this.FileFetcher.GetFileToDownloadAsync(entry.Folder.FolderId, entry.Path);

            return this.PhysicalFile(di.FullName, di.MimeType, di.FullName.Split(Path.DirectorySeparatorChar).LastOrDefault());
        }

        protected async Task<bool> EnsureNonRemoved(SharedEntry entry)
        {
            var exists = await this.FileFetcher.IsValidPath(entry.Folder.FolderId, entry.Path);
            if (exists) return true;
            try
            {
                await this.CommandFactory.Create<StopSharingCommand>().Setup(entry.Id).ExecuteAsync();
            }
            finally
            {
                this.logger.LogWarning(ShareLoggingEvents.InvalidShareLink,
                    "Trying to access entry of id \"{0}\" - file doesn't exist, so link will be removed.", entry.Id);
            }

            return false;
        }
    }
}