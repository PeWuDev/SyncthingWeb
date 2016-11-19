using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace SyncthingWeb.Extensions
{
    
    public static class MenuExtensions
    {
        public static string ActivePage(this IHtmlHelper helper, string controller, string action, string area, object additional = null)
        {
            return null;
            //var classValue = "";

            //var vc = helper.ViewContext;

            //var currentController = vc.Controller.ValueProvider.GetValue("controller").RawValue.ToString();
            //var currentAction = vc.Controller.ValueProvider.GetValue("action").RawValue.ToString();
            //var currentArea = (vc.RequestContext.RouteData.DataTokens["area"] ?? string.Empty).ToString();

            //if (currentController == controller && currentAction == action && currentArea == area)
            //{
            //    var isGood = true;
            //    if (additional != null)
            //    {
            //        var rc = new RouteValueDictionary(additional);
            //        foreach (var pk in rc)
            //        {
            //            var rawVal = vc.Controller.ValueProvider.GetValue(pk.Key)?.RawValue?.ToString();
            //            if (rawVal != pk.Value?.ToString())
            //            {
            //                isGood = false;
            //            }
            //        }
            //    }

            //    if (isGood) classValue = "active";
            //}

            //return classValue;
        }
    }
}