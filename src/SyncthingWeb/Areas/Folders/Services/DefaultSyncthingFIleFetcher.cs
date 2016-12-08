using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.Areas.Folders.Services
{
    public class DefaultSyncthingFileFetcher : ISyncthingFileFetcher
    {
        private readonly ISyncthingContextFactory syncthingContextFactory;

        public DefaultSyncthingFileFetcher(ISyncthingContextFactory syncthingContextFactory)
        {
            this.syncthingContextFactory = syncthingContextFactory;
        }

        public async Task<IEnumerable<FileEntryContext>> GetFilesAsync(string id, string path)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var fd = ctx.GetFolder(id);

            var fullPath = fd.GetFullPath(path);
            
            var dirInfo = new DirectoryInfo(fullPath);
            var files =
                await
                    Task.Factory.StartNew(
                        () => dirInfo.GetFiles().Where(fi => !fi.Attributes.HasFlag(FileAttributes.Hidden)));
            return files.Select(f => new FileEntryContext(f, id, path));
        }

        public async Task<IEnumerable<FileEntryContext>> SearchFilesAsync(IEnumerable<string> id, string pattern)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var folders = ctx.Folders.Where(f => id.Contains(f.Id));

            return await Task.Factory.StartNew(() =>
            {
                var list = new List<FileEntryContext>();
                foreach (var fd in folders)
                    list.AddRange(
                        Directory.EnumerateFiles(fd.Path, "*.*", SearchOption.AllDirectories)
                        .Where(f => Path.GetFileName(f).Contains(pattern))
                            .Select(f => new FileEntryContext(new FileInfo(f), fd.Id, fd.GeRelativePath(f))));

                return list;
            });
        }

        public async Task<IEnumerable<DirectoryEntryContext>> GetDirectoriesAsync(string id, string path)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var fd = ctx.GetFolder(id);

            var fullPath = fd.GetFullPath(path);

            var dirInfo = new DirectoryInfo(fullPath);
            var dirs = await Task.Factory.StartNew(() => dirInfo.GetDirectories());
            return dirs.Select(f => new DirectoryEntryContext(f));
        }

        public async Task<DownloadFileInfo> GetFileToDownloadAsync(string id, string path)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var fd = ctx.GetFolder(id);


            var fullPath = fd.GetFullPath(path);
            return new DownloadFileInfo(new FileInfo(fullPath));
        }

        public async Task<DownloadFileInfo> DownloadableFolder(string id, string path)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var fd = ctx.GetFolder(id);
            var fullPath = fd.GetFullPath(path);

            return await Task.Factory.StartNew(() =>
            {
                var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");

                ZipFile.CreateFromDirectory(fullPath, tempFile, CompressionLevel.Fastest, false);

                return new DownloadFileInfo(new FileInfo(tempFile));
            });
        }

        public IEnumerable<string> SplitIntoFolders(string path)
        {
            return path.Split(Path.PathSeparator);
        }

        public async Task<bool> IsValidPath(string id, string path)
        {
            var ctx = await this.syncthingContextFactory.GetContext();
            var fd = ctx.GetFolder(id);
            var fullPath = fd.GetFullPath(path);

            return await Task.Factory.StartNew(() => File.Exists(fullPath));
        }
    }
}