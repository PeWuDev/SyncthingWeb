using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SyncthingWeb.Authorization;
using SyncthingWeb.Commands;
using SyncthingWeb.Logging;
using SyncthingWeb.Notifications;

namespace SyncthingWeb.Helpers
{
    public class ExtendedController : Controller
    {
        public INotification Notifications { get; set; }
        public ITranManager Transactional { get; set; }
        public ICommandFactory CommandFactory { get; set; }
        public IAuthorizer Authorizer { get; set; }


        public ILogger Logger { get; set; } = NullLogger.Instance;

        protected virtual ITransaction BeginTransaction()
        {
            return this.Transactional.Begin();
        }

        protected virtual ITransaction BeginIsolatedTransaction()
        {
            return this.Transactional.Begin(IsolationLevel.Snapshot);
        }
    }
}
