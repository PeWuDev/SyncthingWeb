using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Syncthing.Integration.Configuration;
using Syncthing.Integration.Watchers;

namespace Syncthing.Integration
{
    public class SyncthingContext
    {
        private readonly SyncthingApiEndpoint apiEndpoint;

        private IConfigWatcher configWatcher;
        private string originalConfigAnswer;

        private SyncthingContext(SyncthingApiEndpoint apiEndpoint)
        {
            this.apiEndpoint = apiEndpoint;
        }

        public static async Task<SyncthingContext> CreateAsync(SyncthingApiEndpoint apiEndpoint)
        {
            var instance = new SyncthingContext(apiEndpoint);
            await instance.ParseAsync();

            return instance;
        }

        public SyncthingContextConfiguration Configuration { get; } = new SyncthingContextConfiguration();

        public ReadOnlyCollection<SyncthingFolder> Folders { get; private set; }
        public ReadOnlyCollection<SyncthingDevice> Devices { get; private set; }

        public IConfigWatcher ConfigWatcher => configWatcher ?? (configWatcher = new InternalConfigWatcher(this));

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
            this.originalConfigAnswer = await this.apiEndpoint.GetRawJsonDataAsync("/rest/system/config");
            dynamic result = JObject.Parse(this.originalConfigAnswer);

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

        private class InternalConfigWatcher : IConfigWatcher
        {
            private readonly SyncthingContext ctx;

            public InternalConfigWatcher(SyncthingContext ctx)
            {
                this.ctx = ctx;
            }

            public async Task<bool> ChangedAsync()
            {
                try
                {
                    var newConfig = await ctx.apiEndpoint.GetRawJsonDataAsync("/rest/system/config");

                    const StringComparison comparisonMode = StringComparison.CurrentCultureIgnoreCase;
                    return
                        string.Compare(ctx.originalConfigAnswer, newConfig, comparisonMode) != 0;
                }
                catch (Exception ex)
                {
                    this.ctx.Configuration.OnErrorCallback(
                        $"Cannot comapre newest config to the old one:\n{ex.Message}\nStackTrace:\n{ex.StackTrace}");
                    return false;
                }
            }
        }
    }
}
