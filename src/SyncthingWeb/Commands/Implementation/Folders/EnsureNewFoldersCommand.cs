using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class EnsureNewFoldersCommand : NonQueryCommand
    {
        private readonly ICache cache;
        private HashSet<string> toAdd = new HashSet<string>();

        public EnsureNewFoldersCommand(ICache cache)
        {
            this.cache = cache;
        }

        public EnsureNewFoldersCommand Setup(params string[] toAdd)
        {
            if (toAdd == null) throw new ArgumentNullException("toAdd");
            if (toAdd.Any(f => f == null)) throw new ArgumentException("One of folder id is null.", nameof(toAdd));

            foreach (var elem in toAdd) this.toAdd.Add(elem);

            return this;
        }

        public override Task ExecuteAsync()
        {
            foreach (var f in this.toAdd)
            {
                var entity = new Folder
                {
                    FolderId = f
                };

                this.Context.Folders.Add(entity);
            }
            this.cache.Signal("all-folders");
                return this.Context.SaveChangesAsync();
        }
    }
}