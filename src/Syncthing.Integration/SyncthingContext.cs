using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Syncthing.Integration.Configuration;

namespace Syncthing.Integration
{
    public class SyncthingContext
    {
        private readonly SyncthingApiEndpoint apiEndpoint;

        private bool parsed;
        private SyncthingContext(SyncthingApiEndpoint apiEndpoint)
        {
            this.apiEndpoint = apiEndpoint;
        }        

        public static async Task<SyncthingContext> CreateAsync(SyncthingApiEndpoint apiEndpoint = null)
        {
            var instance = new SyncthingContext(apiEndpoint);
            await instance.ParseAsync();

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

        internal async Task ParseAsync()
        {
            var result = await this.apiEndpoint.GetDynamicDataAsync("/rest/system/config");

            var devices = new List<SyncthingDevice>();
            foreach (var devNode in result.devices)
            {
                devices.Add(new SyncthingDevice(devNode));
            }

            this.Devices = new ReadOnlyCollection<SyncthingDevice>(devices);

            var devicesMap = this.Devices.ToDictionary(d => d.Id);
            var folders = new List<SyncthingFolder>();
            foreach (var folderNode in result.folders)
            {
                folders.Add(new SyncthingFolder(folderNode, devicesMap));
            }

            this.Folders = new ReadOnlyCollection<SyncthingFolder>(folders);
        }

        public SyncthingDevice GetDevice(string deviceId)
        {
            return this.Devices.FirstOrDefault(d => d.Id == deviceId);
        }

        public enum TestAccessResult
        {
            Ok,
            FileNotExists,
            Unknown
        }
    }
}
