using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;
using System.Linq;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class GetAllowedFoldersCommand : GetCommand<Dictionary<string, AllowedFolder>>
    {
        private readonly ICache cache;

        public GetAllowedFoldersCommand(ICache cache)
        {
            this.cache = cache;
        }

        public string User { get; private set; }

        public GetAllowedFoldersCommand Setup(string user)
        {
            this.User = user;
            return this;
        }

        public override Task<Dictionary<string, AllowedFolder>> GetAsync()
        {
            return this.cache.GetAsync("allowed-folders-" + User, async context =>
            {
                var usr =
                    await
                        this.Context.Users.AsNoTracking()
                            .Include(u => u.Allowed)
                            .Include("Allowed.Folder")
                            .FirstOrDefaultAsync(u => u.Id == User);

                return usr.Allowed.ToDictionary(a => a.Folder.FolderId);
            });
        }
    }
}