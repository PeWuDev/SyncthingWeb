using System;
using System.IO;

namespace SyncthingWeb.Areas.Folders.Models
{
    public class DirectoryEntryContext
    {
        public DirectoryEntryContext(DirectoryInfo di)
        {
            if (di == null) throw new ArgumentNullException(nameof(di));
            this.Name = di.Name;
            this.ModifiedDate = di.LastWriteTimeUtc;
        }

        public DateTime ModifiedDate { get;  }

        public string Name { get; }
    }
}