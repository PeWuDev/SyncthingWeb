using System.Collections.Generic;
using SyncthingWeb.Areas.Share.Providers;
using SyncthingWeb.Helpers;

namespace SyncthingWeb.Areas.Share.Models
{
    public class CommonShareViewModel
    {
        public IEnumerable<IShare> Shares { get; set; }
        public string FolderId { get; set; }
        public string Path { get; set; }

        
        public string GetDirPath()
        {
            return PathHelpers.GetDirPathFromRelativePath(this.Path);
        }
    }
}