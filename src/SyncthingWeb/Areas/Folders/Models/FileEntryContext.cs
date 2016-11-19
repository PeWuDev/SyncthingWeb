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

        public string FolderId { get; }
        public string Path { get; }

        public DateTime ModifiedDate { get;}

        public long Length { get; }

        public string Name { get;}
    }
}