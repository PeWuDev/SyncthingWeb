using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncthing.Integration;

namespace SyncthingWeb.Syncthing
{
    public interface ISyncthingContextFactory
    {
        Task<SyncthingContext> GetContext();

        void Reload();
    }
}
