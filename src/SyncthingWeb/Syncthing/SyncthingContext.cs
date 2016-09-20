using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SyncthingWeb.Syncthing
{
    public class SyncthingContext
    {
        private readonly Stream configFile;

        private bool parsed;
        private SyncthingContext(Stream configFile)
        {
            if (configFile == null) throw new ArgumentNullException(nameof(configFile));
            this.configFile = configFile;
        }

        public static SyncthingContext Create(string configPath)
        {
            using (var fr = new FileStream(configPath, FileMode.Open))
            {
                return Create(fr);
            }
        }

        public static Task<bool> TestAccess(string configPath)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    using (File.Open(configPath, FileMode.Open, FileAccess.Read))
                    {
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static SyncthingContext Create(Stream configStream)
        {
            var instance = new SyncthingContext(configStream);
            instance.Parse();

            return instance;
        }

        public ReadOnlyCollection<SyncthingFolder> Folders { get; private set; }
        public ReadOnlyCollection<SyncthingDevice> Devices { get; private set; }

        public IEnumerable<SyncthingFolder> GetFolders(string deviceId)
        {
            return this.Folders.Where(f => f.Devices.Any(dev => dev.Id == deviceId));
        }

        public SyncthingFolder GetFolder(string id)
        {
            var found = this.Folders.SingleOrDefault(s => s.Id == id);
            if (found == null) throw new InvalidOperationException("Invalid folder path id");

            return found;
        }

        internal void Parse()
        {
            if (parsed) throw new InvalidOperationException("Already parsed.");

            using (var sr = new StreamReader(this.configFile))
            {
                var xdox = XDocument.Load(sr);
                if (xdox.Root == null) throw new InvalidOperationException("Invalid configuration file.");

                this.Devices =
                    new ReadOnlyCollection<SyncthingDevice>(
                        xdox.Root.Elements("device").Select(deviceNode => new SyncthingDevice(deviceNode)).ToList());

                var devicesDictionary = this.Devices.ToDictionary(d => d.Id);
                this.Folders =
                    new ReadOnlyCollection<SyncthingFolder>(
                        xdox.Root.Elements("folder")
                            .Select(folderNode => new SyncthingFolder(folderNode, devicesDictionary))
                            .ToList());
            }
        }

        public SyncthingDevice GetDevice(string deviceId)
        {
            return this.Devices.FirstOrDefault(d => d.Id == deviceId);
        }
    }
}
