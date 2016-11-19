using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Commands;
using SyncthingWeb.Controllers;
using SyncthingWeb.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using HomeController = SyncthingWeb.Areas.Setup.Controllers.HomeController;

namespace SyncthingWeb.Attributes
{
    public class SetupRequiredAttribute : IActionFilter
    {
        private static bool? initializedCache = null;

        private static object globalLock = new object();

        public ICommandFactory CommandFactory { get; set; }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor == null) return;

            if (Equals(actionDescriptor.ControllerTypeInfo, typeof( HomeController).GetTypeInfo()))
            {
                return;
            }

            var initialized = IsInitialized();
            if (initialized) return;

            context.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary { { "action", "Index" }, { "controller", "Home" }, { "area", "Setup" } });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public static bool IsInitialized()
        {
            if (initializedCache != null) return initializedCache.Value;

            lock (globalLock)
            {
                using (var ctx = Startup.ApplicationContainer.Resolve<ApplicationDbContext>())
                {
                    var ent = ctx.GeneralSettingses.AsNoTracking().FirstOrDefault();
                    if (ent == null) return false;

                    if (!ent.Initialzed) return false;
                    initializedCache = true;
                    return true;
                }
            }
        }
    }
}
