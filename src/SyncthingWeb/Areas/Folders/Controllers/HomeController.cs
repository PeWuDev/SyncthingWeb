using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Folders.Helpers;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Areas.Folders.Services;
using SyncthingWeb.Commands.Implementation.SharedEntries;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Folders.Controllers
{
    //TODO session state readonly
    [Area("Folders")]
    public class HomeController : FolderControllerBase
    {
        private readonly ISyncthingFileFetcher fileFetcher;

        public HomeController(ISyncthingFileFetcher fileFetcher)
        {
            this.fileFetcher = fileFetcher;
        }

        public async Task<ActionResult> Index(string id, string path = "")
        {
            if (!await this.HasAccess(id))
            {
                return new UnauthorizedResult();
            }

            if (!await this.Exists(id)) return this.NotFound();

            var files = await this.fileFetcher.GetFilesAsync(id, path);
            var dirs = await this.fileFetcher.GetDirectoriesAsync(id, path);

            var filesEvaluated = files as IList<FileEntryContext> ?? files.ToList();
            var shared =
                await
                    this.CommandFactory.Create<GetSharedEntriesForPathCommand>()
                        .Setup(id, this.UserManager.GetUserId(this.User), filesEvaluated.Select(f => Path.Combine(path, f.Name)).ToArray()).GetAsync();

            var wm = new StorageContentViewModel(dirs, filesEvaluated) { FolderId = id, CurrPath = path };

            this.BuildBreadcrumb(id, path);

            this.ViewBag.Allowed = await this.GetAllowed(id);
            this.ViewBag.CanShare = true; //TODO await this.Authorizer.AuthorizeAsync(SharePermissions.Allow);
            this.ViewBag.Shared = shared;
            return this.View(wm);
        }
        
        public async Task<ActionResult> GetFile(string id, string path)
        {
            if (!await this.HasAccess(id))
            {
                return new UnauthorizedResult();
            }

            if (!await this.Exists(id)) return this.NotFound();

            var downloadableFile = await this.fileFetcher.GetFileToDownloadAsync(id, path);

            return this.PhysicalFile(downloadableFile.FullName, downloadableFile.MimeType,
                path.Split(Path.DirectorySeparatorChar).Last());
        }

        public  async Task<ActionResult> GetFolder(string id, string path)
        {
            if (!await this.HasAccess(id))
            {
                return new UnauthorizedResult();
            }

            if (!await this.Exists(id)) return this.NotFound();

            var zipFolder = await this.fileFetcher.DownloadableFolder(id, path);

            return this.PhysicalFile(zipFolder.FullName, zipFolder.MimeType, (path.Split(Path.DirectorySeparatorChar).LastOrDefault() ?? id) + ".zip");
        }

        private void BuildBreadcrumb(string id, string path)
        {
            var list = new List<BreadcrumbViewModel>
            {
                new BreadcrumbViewModel
                {
                    Text = id,
                    Url = this.Url.Action("Index", new {id}),
                    Icon = "folder"
                }
            };


            if (!string.IsNullOrWhiteSpace(path))
            {
                var aggregator = new StringBuilder();
                foreach (var part in this.fileFetcher.SplitIntoFolders(path))
                {
                    if (aggregator.Length > 0) aggregator.Append(Path.DirectorySeparatorChar);
                    aggregator.Append(part);

                    list.Add(new BreadcrumbViewModel
                    {
                        Url = this.Url.Action("Index", new { id, path = aggregator.ToString() }),
                        Text = part
                    });
                }
            }

            this.ViewBag.Breadcrumb = list;
        }
    }
}
