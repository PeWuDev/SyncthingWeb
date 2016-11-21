using System.Collections.Generic;
using Syncthing.Integration;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.Models
{
    public class DashboardViewModel
    {
        public int Folders { get; set; }
        public int Shared { get; set; }

        public IEnumerable<SyncthingFolder> FoldersId { get; set; }
    }
}