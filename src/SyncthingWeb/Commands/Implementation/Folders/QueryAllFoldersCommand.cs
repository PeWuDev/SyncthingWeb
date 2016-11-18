using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;
using  System.Linq;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class QueryAllFoldersCommand : QueryCommand<Folder>
    {
        private readonly ICache cache;

        public QueryAllFoldersCommand(ICache cache)
        {
            this.cache = cache;
        }

        public override async Task<IEnumerable<Folder>> ExecuteAsync()
        {
            if (this.IgnoreCache) return (await this.Context.Folders.AsNoTracking().ToListAsync()).AsEnumerable();

            return await this.cache.GetAsync("all-folders",
                async context => (await this.Context.Folders.AsNoTracking().ToListAsync()).AsEnumerable());
        }

        public QueryAllFoldersCommand NoCache()
        {
            this.IgnoreCache = true;
            return this;
        }

        public bool IgnoreCache { get; set; }
    }
}