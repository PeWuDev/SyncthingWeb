using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace SyncthingWeb.Areas.Folders.Models
{
    public class DownloadFileInfo
    {
        private readonly FileInfo fileInfo;

        public DownloadFileInfo(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public string FullName => this.fileInfo.FullName;

        public string MimeType
        {
            get
            {
                string result;
                new FileExtensionContentTypeProvider().TryGetContentType(this.FullName, out result);

                return result ?? "application/octet-stream";
            }
        } 
    }
}