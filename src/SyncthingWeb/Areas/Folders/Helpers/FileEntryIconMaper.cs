using System.Collections.Generic;
using System.IO;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Helpers
{
    public static class FileEntryIconMaper
    {
        private static readonly Dictionary<string, string> map;

        static FileEntryIconMaper()
        {
            map = new Dictionary<string, string>
            {
                {"xlsx", "file-excel-o"},
                {"xls", "file-excel-o"},
                {"pdf", "file-pdf-o"},
                {"mp3", "file-sound-o"},
                {"wav", "file-sound-o"},
                {"ogg", "file-sound-o"},
                {"flac", "file-sound-o"},
                {"aac", "file-sound-o"},
                {"docx", "file-word-o"},
                {"doc", "file-word-o"},
                {"odt", "file-word-o"},
                {"ppt", "file-powerpoint-o"},
                {"pptx", "file-powerpoint-o"},
                {"odp", "file-powerpoint-o"},
                {"zip", "file-archive-o"},
                {"rar", "file-archive-o"},
                {"7z", "file-archive-o"},
                {"gz", "file-archive-o"},
                {"tar", "file-archive-o"},
                {"cab", "file-archive-o"},
                {"bz2", "file-archive-o"},
                {"jpg", "file-image-o"},
                {"jpeg", "file-image-o"},
                {"png", "file-image-o"},
                {"bmp", "file-image-o"},
                {"gif", "file-image-o"},
                {"tiff", "file-image-o"},
                {"raw", "file-image-o"},
                {"svg", "file-image-o"},
                {"xcf", "file-image-o"},
                {"txt", "file-text-o"},
                {"config", "file-text-o"},
                {"avi", "file-movie-o"},
                {"divx", "file-movie-o"},
                {"mpg", "file-movie-o"},
                {"mpeg", "file-movie-o"},
                {"wmv", "file-movie-o"},
                {"mkv", "file-movie-o"},
                {"ogv", "file-movie-o"},
                {"mov", "file-movie-o"},
                {"rm", "file-movie-o"},
                {"rmvb", "file-movie-o"},
                {"mp4", "file-movie-o"},
                {"3fp", "file-movie-o"},
                {"flv", "file-movie-o"},
                {"cs", "file-code-o"},
                {"java", "file-code-o"},
                {"cpp", "file-code-o"},
                {"c", "file-code-o"},
                {"fs", "file-code-o"},
                {"ps", "file-code-o"},
                {"ini", "file-code-o"},
                {"xaml", "file-code-o"},
                {"xml", "file-code-o"},
                {"json", "file-code-o"},
                {"vb", "file-code-o"},
                {"sh", "file-code-o"},
                {"md", "file-code-o"},
                {"js", "file-code-o"},
                {"php", "file-code-o"},
                {"css", "file-code-o"},
                {"rb", "file-code-o"},
                {"html", "file-code-o"}
            };
        }

        public static string Map(FileEntryContext ctx)
        {
            var ext = Path.GetExtension(ctx.Name).ToLowerInvariant().TrimStart('.');
            if (string.IsNullOrWhiteSpace(ext) || !map.ContainsKey(ext)) return "file-o";

            return map[ext];
        }
    }
}