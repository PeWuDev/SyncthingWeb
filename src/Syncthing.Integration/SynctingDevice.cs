using System.Collections.Generic;
using System.Xml.Linq;

namespace Syncthing.Integration
{
    public class SyncthingDevice
    {
        private readonly HashSet<SyncthingFolder> folders;

        internal SyncthingDevice(dynamic apiConfig)
        {
            this.Id = apiConfig.deviceID;
            this.Name = apiConfig.name;
            this.Compression = apiConfig.compression;
            this.folders = new HashSet<SyncthingFolder>();
        }

        public string Name { get; set; }

        public string Id { get; private set; }

        public string Compression { get; private set; }

        //TODO readonly collection
        public IEnumerable<SyncthingFolder> Folders => this.folders;

        internal bool PutFolder(SyncthingFolder folder)
        {
            return this.folders.Add(folder);
        }
    }
}
