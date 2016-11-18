using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class AssignAllowedCommand: NonQueryCommand
    {
        private readonly ICache cache;

        public AssignAllowedCommand(ICache cache)
        {
            this.cache = cache;
        }

        public AllowedParam[] Folders { get; private set; }
        public string User { get; private set; }


        public AssignAllowedCommand Setup(string user, params AllowedParam[] allowed)
        {
            this.Folders = allowed;
            this.User = user;
            return this;
        }

        public override async Task ExecuteAsync()
        {
            var fdId = this.Folders.Select(fd => fd.FolderId).ToArray();

            var allFolders = await this.Context.Folders.Where(fd => fdId.Contains(fd.FolderId)).ToListAsync();
            var usr = await this.Context.Users.SingleOrDefaultAsync(u => u.Id == this.User);

            usr.Allowed.Clear();
            foreach (var fd in allFolders)
                usr.Allowed.Add(new AllowedFolder
                {
                    FolderId = fd.Id
                });

            try
            {
                await this.Context.SaveChangesAsync();
            }
            finally
            {
                this.cache.Signal("allowed-folders-" + this.User);
            }
        }

        public class AllowedParam
        {
            public AllowedParam(string folderId)
            {
                FolderId = folderId;
            }

            public string FolderId { get; }
        }
    }
}