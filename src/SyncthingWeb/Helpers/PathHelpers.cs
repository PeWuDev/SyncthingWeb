namespace SyncthingWeb.Helpers
{
    using System.IO;
    using System.Linq;

    public class PathHelpers
    {
        public static string GetDirPathFromRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;

            if (!path.Contains(Path.DirectorySeparatorChar.ToString())) return string.Empty;

            return string.Join(Path.DirectorySeparatorChar.ToString(),
                path.Split(Path.DirectorySeparatorChar).Reverse().Skip(1).Reverse());
        }
    }
}