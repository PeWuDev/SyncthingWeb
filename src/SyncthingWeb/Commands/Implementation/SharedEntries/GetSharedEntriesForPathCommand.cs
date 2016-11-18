using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.SharedEntries
{
    public class GetSharedEntriesForPathCommand : GetCommand<Dictionary<string, SharedEntry>>
    {
        public string User { get; private set; }
        public string Id { get; private set; }
        public string[] Paths { get; private set; }

        public GetSharedEntriesForPathCommand Setup(string id, string user, params string[] paths)
        {
            this.User = user;
            this.Id = id;
            this.Paths = paths;
            return this;
        }

        public override async Task<Dictionary<string, SharedEntry>> GetAsync()
        {
            var r = await
                this.Context.SharedEntries.Include(sh => sh.Folder)
                    .Where(
                        sh => sh.OwnerId == this.User && sh.Folder.FolderId == this.Id && this.Paths.Contains(sh.Path))
                    .GroupBy(sh => sh.Path).ToListAsync();

            return r.ToDictionary(sh => sh.Key, sh => sh.SingleOrDefault());
        }
    }
}