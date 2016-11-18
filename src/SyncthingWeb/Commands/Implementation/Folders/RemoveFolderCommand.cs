using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class RemoveFolderCommand : NonQueryCommand
    {
        private readonly ICache cache;
        private readonly HashSet<string> toRemove = new HashSet<string>();

        public RemoveFolderCommand(ICache cache)
        {
            this.cache = cache;
        }

        public RemoveFolderCommand Setup(params string[] names)
        {
            if (names == null) throw new ArgumentNullException("names");
            if (names.Any(f => f == null)) throw new ArgumentException("One of folder id is null.", nameof(names));

            foreach (var elem in names) this.toRemove.Add(elem);

            return this;
        }

        public override async Task ExecuteAsync()
        {
            if (!this.toRemove.Any()) return;

            var allToRemove = await this.Context.Folders.Where(f => this.toRemove.Contains(f.FolderId)).ToListAsync();

            foreach (var entry in this.Context.SharedEntries.Where(se => this.toRemove.Contains(se.Folder.FolderId)))
            {
                this.Context.SharedEntries.Remove(entry);
            }

            await this.Context.SaveChangesAsync();

            foreach (var entity in allToRemove)
            {
                entity.Allowed.Clear();
                entity.Shared.Clear();
                this.Context.Folders.Remove(entity);
            }

            this.cache.Signal("all-folders");
            await this.Context.SaveChangesAsync();
        }
    }
}