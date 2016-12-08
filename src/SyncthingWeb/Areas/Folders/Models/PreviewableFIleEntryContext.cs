using System.IO;
using SyncthingWeb.Areas.Folders.Preview;

namespace SyncthingWeb.Areas.Folders.Models
{
    public class PreviewableFileEntryContext : FileEntryContext
    {
        public PreviewableFileEntryContext(FileEntryContext context, PreviewProvider previewProvider)
        {
            this.Name = context.Name;
            this.Length = context.Length;
            this.ModifiedDate = context.ModifiedDate;
            this.FolderId = context.FolderId;
            this.Path = context.Path;

            this.PreviewProvider = previewProvider;
        }

        public PreviewableFileEntryContext(FileInfo file, string id, string path, PreviewProvider previewProvider) : base(file, id, path)
        {
            PreviewProvider = previewProvider;
        }

        public PreviewProvider PreviewProvider { get; }

    }
}
