using System.Collections.Generic;
using System.IO;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Helpers
{
    public static class FileEntryIconMaper
    {
        private static Dictionary<string, string> map;

        static FileEntryIconMaper()
        {
            map = new Dictionary<string, string>();

            map.Add("xlsx", "file-excel-o");
            map.Add("xls", "file-excel-o");

            map.Add("pdf", "file-pdf-o");

            map.Add("mp3", "file-sound-o");
            map.Add("wav", "file-sound-o");
            map.Add("ogg", "file-sound-o");
            map.Add("flac", "file-sound-o");
            map.Add("aac", "file-sound-o");

            map.Add("docx", "file-word-o");
            map.Add("doc", "file-word-o");
            map.Add("odt", "file-word-o");

            map.Add("ppt", "file-powerpoint-o");
            map.Add("pptx", "file-powerpoint-o");
            map.Add("odp", "file-powerpoint-o");

            map.Add("zip", "file-archive-o");
            map.Add("rar", "file-archive-o");
            map.Add("7z", "file-archive-o");
            map.Add("gz", "file-archive-o");
            map.Add("tar", "file-archive-o");
            map.Add("cab", "file-archive-o");
            map.Add("bz2", "file-archive-o");

            map.Add("jpg", "file-image-o");
            map.Add("jpeg", "file-image-o");
            map.Add("png", "file-image-o");
            map.Add("bmp", "file-image-o");
            map.Add("gif", "file-image-o");
            map.Add("tiff", "file-image-o");
            map.Add("raw", "file-image-o");
            map.Add("svg", "file-image-o");
            map.Add("xcf", "file-image-o");

            map.Add("txt", "file-text-o");
            map.Add("config", "file-text-o");

            map.Add("avi", "file-movie-o");
            map.Add("divx", "file-movie-o");
            map.Add("mpg", "file-movie-o");
            map.Add("mpeg", "file-movie-o");
            map.Add("wmv", "file-movie-o");
            map.Add("mkv", "file-movie-o");
            map.Add("ogv", "file-movie-o");
            map.Add("mov", "file-movie-o");
            map.Add("rm", "file-movie-o");
            map.Add("rmvb", "file-movie-o");
            map.Add("mp4", "file-movie-o");
            map.Add("3fp", "file-movie-o");
            map.Add("flv", "file-movie-o");

            map.Add("cs", "file-code-o");
            map.Add("java", "file-code-o");
            map.Add("cpp", "file-code-o");
            map.Add("c", "file-code-o");
            map.Add("fs", "file-code-o");
            map.Add("ps", "file-code-o");
            map.Add("ini", "file-code-o");
            map.Add("xaml", "file-code-o");
            map.Add("xml", "file-code-o");
            map.Add("json", "file-code-o");
            map.Add("vb", "file-code-o");
            map.Add("sh", "file-code-o");
            map.Add("md", "file-code-o");
            map.Add("js", "file-code-o");
            map.Add("php", "file-code-o");
            map.Add("css", "file-code-o");
            map.Add("rb", "file-code-o");
            map.Add("html", "file-code-o");
        }

        public static string Map(FileEntryContext ctx)
        {
            var ext = Path.GetExtension(ctx.Name).ToLowerInvariant().TrimStart('.');
            if (string.IsNullOrWhiteSpace(ext) || !map.ContainsKey(ext)) return "file-o";

            return map[ext];
        }
    }
}