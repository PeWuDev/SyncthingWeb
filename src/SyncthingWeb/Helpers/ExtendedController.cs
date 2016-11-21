using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SyncthingWeb.Authorization;
using SyncthingWeb.Commands;
using SyncthingWeb.Notifications;

namespace SyncthingWeb.Helpers
{
    public class ExtendedController : Controller
    {
        public INotification Notifications => (INotification)this.HttpContext.RequestServices.GetService(typeof(INotification));
        public ITranManager Transactional => (ITranManager)this.HttpContext.RequestServices.GetService(typeof(ITranManager));
        public ICommandFactory CommandFactory=> (ICommandFactory)this.HttpContext.RequestServices.GetService(typeof(ICommandFactory));
        public IAuthorizer Authorizer => (IAuthorizer)this.HttpContext.RequestServices.GetService(typeof(IAuthorizer));
        
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
