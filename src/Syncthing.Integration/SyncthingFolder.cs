using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Syncthing.Integration
{
    public class SyncthingFolder
    {
        public string Id { get; }

        public string Path { get; }

        public IEnumerable<SyncthingDevice> Devices { get; }

        internal SyncthingFolder(dynamic node, IDictionary<string, SyncthingDevice> devices)
        {
            this.Id = node.id;
            this.Path = node.path;

            var devicesNode = node.devices;

            var devicesLocal = new List<SyncthingDevice>();
            foreach (var devNode in devicesNode)
            {
                string devNodeId = devNode.deviceID;
                devicesLocal.Add(devices[devNodeId]);
            }

            this.Devices = devicesLocal;
            
            foreach (var device in Devices) device.PutFolder(this);
        }

        public string GetFullPath(string relative)
        {
            return System.IO.Path.Combine(this.Path, relative);
        }

        public string GetFullPath(string relative, string fileName)
        {
            return System.IO.Path.Combine(this.Path, relative, fileName);
        }

        public string GeRelativePath(string fileName)
        {
            if (!fileName.StartsWith(this.Path)) throw new InvalidOperationException("Wrong file");

            var directory = System.IO.Path.GetDirectoryName(fileName);
            if (directory == null) throw new InvalidOperationException("Wrong file");

            return directory.Substring(this.Path.TrimEnd(System.IO.Path.DirectorySeparatorChar).Length);
        }
    }
}
