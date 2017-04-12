using System;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Preview.Providers
{
    public class ImagePreviewProvider : PreviewProvider
    {
        public override Guid Id { get; } = Guid.Parse("{B168D7B0-8F4A-45FE-9585-57D702A3BB71}");
        
        public override string Title { get; } = "Photos";
        public override string ItemIconPreviewComponent { get; set; } = "ImageItemPreview";

        public override bool CanPreview(FileEntryContext entry)
        {
            var fn = entry.Name;

            return fn.EndsWith(".jpg") || fn.EndsWith(".jpeg") || fn.EndsWith(".png") ||
                   fn.EndsWith(".gif");
        }
    }
}
