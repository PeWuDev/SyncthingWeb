using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.SharedEntries
{
    public class WriteOrOverrideSharedEntryCommand :  GetCommand<int>
    {
        public string User { get; private set; }
        public string FolderId { get; private set; }
        public string Path { get; private set; }
        public Guid Provider { get; private set; }
        public string Data { get; private set; }

        public WriteOrOverrideSharedEntryCommand Setup(string fdId, string user, string path, Guid provider, string data)
        {
            this.FolderId = fdId;
            this.User = user;
            this.Path = path;
            this.Provider = provider;
            this.Data = data;
            return this;
        }
        

        public override async Task<int> GetAsync()
        {
            var existing =
                await
                    this.Context.SharedEntries.SingleOrDefaultAsync(
                        ent => ent.Folder.FolderId == this.FolderId && ent.Path == Path);

            if (existing == null)
            {
                existing = new SharedEntry();
                this.Context.SharedEntries.Add(existing);
            }

            var folderId =
                (await this.CommandFactory.Create<QueryAllFoldersCommand>().ExecuteAsync()).Single(
                    fd => fd.FolderId == FolderId);

            existing.FolderId = folderId.Id;
            existing.Path = this.Path;
            existing.Provider = this.Provider;
            existing.Data = this.Data;
            existing.ShareTime = DateTime.Now;
            existing.OwnerId = this.User;

            await this.Context.SaveChangesAsync();

            return existing.Id;
        }
    }
}