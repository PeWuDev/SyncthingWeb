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
        private static bool checkForUpgrade = true;

        private static readonly object globalLock = new object();

        public ICommandFactory CommandFactory { get; set; }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor == null) return;

            if (Equals(actionDescriptor.ControllerTypeInfo, typeof(HomeController).GetTypeInfo()))
            {
                return;
            }

            var initialized = IsInitialized();
            if (initialized)
            {
                if (!NeedSettingsUpgrade()) return;


                context.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"action", "SafeUpgrade"},
                            {"controller", "Home"},
                            {"area", "Setup"}
                        });

                return;
            }

            context.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary {{"action", "Index"}, {"controller", "Home"}, {"area", "Setup"}});
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public static bool NeedSettingsUpgrade()
        {
            //no need to check
            if (!checkForUpgrade) return false;

            //upgrade not need when not initialized
            if (!IsInitialized()) return false;

            lock (globalLock)
            {
                var ctx = Startup.ApplicationContainer.Resolve<ApplicationDbContext>();
                {
                    //must enttiy exists (IsInitialized ensured that);
                    var ent = ctx.GeneralSettingses.AsNoTracking().First();

                    if (ent.NeedsUpgrade()) return true;

                    checkForUpgrade = false;
                    return false;
                }
            }
        }

        public static bool IsInitialized()
        {
            if (initializedCache != null) return initializedCache.Value;

            lock (globalLock)
            {
                var ctx = Startup.ApplicationContainer.Resolve<ApplicationDbContext>();
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
