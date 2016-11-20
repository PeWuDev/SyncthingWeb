using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SyncthingWeb.Areas.Folders.Helpers;
using SyncthingWeb.Areas.Folders.Services;
using SyncthingWeb.Bus;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Searching;

namespace SyncthingWeb.Areas.Folders.Directors
{
    public class FileSearchDirectory : IEventHandler<ISearchCollector>
    {
        private readonly ISyncthingFileFetcher fileFetcher;
        private readonly ICommandFactory commandFactory;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public FileSearchDirectory(ISyncthingFileFetcher fileFetcher, ICommandFactory commandFactory, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.fileFetcher = fileFetcher;
            this.commandFactory = commandFactory;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        public async Task HandleAsync(ISearchCollector @event)
        {
            var allowed = await this.commandFactory.Create<GetAllowedFoldersCommand>().Setup(@event.User).GetAsync();


            var urlHelper = new UrlHelper(this.actionContextAccessor.ActionContext);
            foreach (var fe in await this.fileFetcher.SearchFilesAsync(allowed.Keys.ToList(), @event.Term))
            {
                var url = urlHelper.Action(new UrlActionContext
                {
                    Controller = "Home",
                    Action = "Index",
                    Values = new { area = "Folders", id = fe.FolderId, path = fe.Path }
                });
                @event.Add(new SearchResultItem(fe.Name, fe.FolderId, FileEntryIconMaper.Map(fe), "bg-yellow", url));
            }

        }
    }
}