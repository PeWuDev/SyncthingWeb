using System.Collections.Generic;
using System.Xml.Linq;

namespace Syncthing.Integration
{
    public class SyncthingDevice
    {
        private readonly HashSet<SyncthingFolder> folders;

        internal SyncthingDevice(XElement node)
        {
            this.Id = node.Attribute("id").Value;
            this.Name = node.Attribute("name").Value;
            this.Compression = node.Attribute("compression")?.Value;
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
