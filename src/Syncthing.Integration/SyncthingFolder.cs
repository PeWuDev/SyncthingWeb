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

        internal SyncthingFolder(XElement node, IDictionary<string, SyncthingDevice> devices)
        {

            this.Id = node.Attribute("id").Value;
            this.Path = node.Attribute("path").Value;

            var devicesNode = node.Elements("device");

            this.Devices = (from devNode in devicesNode
                            select devNode.Attribute("id").Value
                                into devId
                            where devices.ContainsKey(devId)
                            select devices[devId]).ToList();

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
