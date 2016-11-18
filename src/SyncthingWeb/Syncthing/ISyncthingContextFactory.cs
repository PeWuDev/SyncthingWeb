using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Syncthing
{
    public interface ISyncthingContextFactory
    {
        Task<SyncthingContext> GetContext();

        void Reload();
    }
}
