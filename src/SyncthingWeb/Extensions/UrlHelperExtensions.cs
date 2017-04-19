using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Settings;

namespace SyncthingWeb.Extensions
{
    
/// <summary>
/// <see cref="IUrlHelper"/> extension methods.
/// </summary>
    public static class UrlHelperExtensions
    {
        public static string AbsoluteOrCustomAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            var commandFactory = (ICommandFactory)url.ActionContext.HttpContext.RequestServices.GetService(typeof(ICommandFactory));
            var settings = Task.Run(
                    async () => await commandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync())
                .Result;
            
            
            if (BuildAbsoluteUrl(settings.RootUrl, url.Action(actionName, controllerName, routeValues), out var result))
            {
                return result;
            }

            return AbsoluteAction(url, actionName, controllerName, routeValues);
        }

        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
        /// route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            return url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
        /// virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteContent(
            this IUrlHelper url,
            string contentPath)
        {
            HttpRequest request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified route by using the route name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteRouteUrl(
            this IUrlHelper url,
            string routeName,
            object routeValues = null)
        {
            return url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }



        private static bool BuildAbsoluteUrl(string rootUrl, string partUrl, out string result)
        {
            if (string.IsNullOrWhiteSpace(rootUrl))
            {
                result = null;
                return false;
            }

            if (!Uri.TryCreate(rootUrl, UriKind.Absolute, out Uri rootUri))
            {
                result = null;
                return false;
            }

            var mainUrl = rootUri.GetComponents(
                UriComponents.Scheme | UriComponents.Host | UriComponents.SchemeAndServer,
                UriFormat.Unescaped);

            result = mainUrl + (partUrl.StartsWith("/") ? "" : "/") + partUrl;
            return true;
        }
    }
}
