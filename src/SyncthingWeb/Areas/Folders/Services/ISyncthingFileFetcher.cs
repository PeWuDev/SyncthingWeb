using System.Collections.Generic;
using System.Threading.Tasks;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Services
{
    public interface ISyncthingFileFetcher
    {
        Task<IEnumerable<FileEntryContext>> GetFilesAsync(string id, string path);
        Task<IEnumerable<FileEntryContext>> SearchFilesAsync(IEnumerable<string> id, string pattern);

        Task<IEnumerable<DirectoryEntryContext>> GetDirectoriesAsync(string id, string path);

        Task<DownloadFileInfo> GetFileToDownloadAsync(string id, string path);

        Task<DownloadFileInfo> DownloadableFolder(string id, string path);

        IEnumerable<string> SplitIntoFolders(string path);

        Task<bool> IsValidPath(string id, string path);
    }
}
