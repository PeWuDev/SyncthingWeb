using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Settings
{
    public class GetCurrentGeneralSettingsCommand : GetCommand<GeneralSettings>
    {
        private readonly ICache cache;

        public GetCurrentGeneralSettingsCommand(ICache cache)
        {
            this.cache = cache;
            this.NoCache = false;
        }

        public bool NoCache { get; private set; }

        public GetCurrentGeneralSettingsCommand SetupNoCache(bool nocache)
        {
            this.NoCache = nocache;
            return this;
        }



        public override Task<GeneralSettings> GetAsync()
        {
            return this.NoCache
                       ? this.GetNoCache()
                       : this.cache.GetAsync("general-settings", context => this.GetNoCache());
        }

        private async Task<GeneralSettings> GetNoCache()
        {
            var current = (await this.Context.GeneralSettingses.AsNoTracking().ToArrayAsync()).SingleOrDefault();
            if (current != null) return current;

            current = new GeneralSettings();
            this.Context.GeneralSettingses.Add(current);
            await this.Context.SaveChangesAsync();

            return current;
        }
    }
}
