using Microsoft.AspNetCore.Routing;

namespace SyncthingWeb.Extensions.AdminLteUIOptions
{
    public class PagerBuilder : IPagerBuilder
    {
        public RouteValueDictionary AdditionalRoutes { get; private set; }
        public IPagerBuilder WithRouteValues(object routeValues)
        {
            this.AdditionalRoutes = new RouteValueDictionary(routeValues);
            return this;
        }
    }
}