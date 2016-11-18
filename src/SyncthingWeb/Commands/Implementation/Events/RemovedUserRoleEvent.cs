using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Commands.Implementation.Events
{
    public class RemovedUserRoleEvent
    {
        public RemovedUserRoleEvent(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; private set; }
    }
}
