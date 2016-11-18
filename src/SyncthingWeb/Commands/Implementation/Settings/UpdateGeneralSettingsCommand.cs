using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Models;
using SyncthingWeb.Syncthing;

namespace SyncthingWeb.Commands.Implementation.Settings
{
    public class UpdateGeneralSettingsCommand : NonQueryCommand
    {
        private readonly ICache cache;
        private LinkedList<Action<GeneralSettings>> delayed = new LinkedList<Action<GeneralSettings>>();

        private readonly ISyncthingContextFactory scf;

        public UpdateGeneralSettingsCommand(ICache cache, ISyncthingContextFactory scf)
        {
            this.cache = cache;
            this.scf = scf;
        }

        public UpdateGeneralSettingsCommand Setup(Action<GeneralSettings> action)
        {
            this.delayed.AddLast(action);
            return this;
        }

        public override async Task ExecuteAsync()
        {
            var entity = await this.Context.GeneralSettingses.FirstOrDefaultAsync();
            if (entity == null)
            {
                entity = new GeneralSettings();
                this.Context.GeneralSettingses.Add(entity);
            }
            else
            {
                this.Context.GeneralSettingses.Attach(entity);
            }

            foreach (var act in this.delayed) act(entity);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            finally
            {
                this.cache.Signal("general-settings");
                this.scf.Reload();
            }
        }

    }
}
