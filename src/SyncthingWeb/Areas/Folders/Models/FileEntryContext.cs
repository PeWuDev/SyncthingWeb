using System;
using System.IO;

namespace SyncthingWeb.Areas.Folders.Models
{
    public class FileEntryContext
    {
        public FileEntryContext(FileInfo file, string id, string path)
        {
            if (file == null) throw new ArgumentNullException();

            this.Name = file.Name;
            this.Length = file.Length;
            this.ModifiedDate = file.LastWriteTimeUtc;
            this.FolderId = id;
            this.Path = path;
        }

        protected FileEntryContext()
        {
        }

        public string FolderId { get; protected set; }
        public string Path { get; protected set; }

        public DateTime ModifiedDate { get; protected set; }

        public long Length { get; protected set; }

        public string Name { get; protected set; }
    }
}