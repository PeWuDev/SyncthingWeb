using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SyncthingWeb.Extensions.AdminLteUIOptions;

namespace SyncthingWeb.Extensions
{
    public static class AdminLteUIExtensions
    {
        public static AdminLteUIBuilder<TModel> AdminLteUI<TModel>(this IHtmlHelper<TModel> htmlHelper)
        {
            return new AdminLteUIBuilder<TModel>(htmlHelper);
        }
    }

    public class AdminLteUIBuilder<TModel>
    {
        private readonly IHtmlHelper<TModel> htmlHelper;

        public AdminLteUIBuilder(IHtmlHelper<TModel> htmlHelper)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            this.htmlHelper = htmlHelper;
        }

        public HtmlString Pager(string actioName, string controllerName, object routeValues, int page, int pageSize, int totalItems, Action<IPagerBuilder> options = null)
        {
            var pagerOptions = new PagerBuilder();
            options?.Invoke(pagerOptions);

            var rootUlElement = new TagBuilder("ul");
            rootUlElement.AddCssClass("pagination");

            var howMuchPagesPerPager = 5;

            var urlHelper = new UrlHelper(this.htmlHelper.ViewContext);

            var mergedRouteValues = new RouteValueDictionary(routeValues ?? new {});
            foreach (var mergedRouteValue in mergedRouteValues.ToList())
            {
                if (mergedRouteValue.Key.ToLowerInvariant() == "page") mergedRouteValues.Remove(mergedRouteValue.Key);
            }

            foreach (
                var additionalRoute in pagerOptions.AdditionalRoutes ?? Enumerable.Empty<KeyValuePair<string, Object>>()
                )
            {
                if (additionalRoute.Key.ToLowerInvariant() == "page") continue;

                mergedRouteValues.Add(additionalRoute.Key, additionalRoute.Value);
            }


            Func<int, string> buildUrlForPage = pageNo =>
            {
                var finalRoute = new RouteValueDictionary(mergedRouteValues) {{"page", pageNo}};

                return urlHelper.Action(new UrlActionContext
                {
                    Action = actioName,
                    Controller = controllerName,
                    Values = finalRoute
                });
            };

            var totalPages = totalItems/pageSize + 1;
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;
            var pagesToProcess = new LinkedList<int>();


            var firstPage = Math.Max(page - howMuchPagesPerPager/2, 1);
            for (var i = firstPage; i < page; i++)
            {
                pagesToProcess.AddLast(i);
            }

            pagesToProcess.AddFirst(page);

            var lastPage = Math.Min(totalPages, page + howMuchPagesPerPager/2);
            for (var i = page + 1; i < lastPage; i++)
            {
                pagesToProcess.AddLast(i);
            }

            while (pagesToProcess.Count < howMuchPagesPerPager && lastPage < totalPages)
            {
                lastPage += 1;
                pagesToProcess.AddLast(lastPage);
            }

            while (pagesToProcess.Count < howMuchPagesPerPager && firstPage>  1)
            {
                lastPage -= 1;
                pagesToProcess.AddFirst(lastPage);
            }

            var prevButton = new TagBuilder("li");
            prevButton.AddCssClass("paginate_button");
            prevButton.AddCssClass("previous");
            var aPrevButton = new TagBuilder("a");
            aPrevButton.InnerHtml.SetContent("Previous");
            if (page > 1)
            {
                aPrevButton.Attributes["href"] = buildUrlForPage(page - 1);
            }
            else
            {
                prevButton.AddCssClass("disabled");
            }

             prevButton.InnerHtml.SetHtmlContent(aPrevButton);
            rootUlElement.InnerHtml.AppendHtml(prevButton);


            foreach (var pageToProcess in pagesToProcess)
            {
                var li = new TagBuilder("li");
                li.AddCssClass("paginate_button");
                if (pageToProcess == page)
                    li.AddCssClass("active");

                var a = new TagBuilder("a");
                a.Attributes["href"] = buildUrlForPage(pageToProcess);
                a.InnerHtml.SetContent(pageToProcess.ToString("D"));

                li.InnerHtml.SetHtmlContent(a);

                rootUlElement.InnerHtml.AppendHtml(li);
            }


            var nextButton = new TagBuilder("li");
            nextButton.AddCssClass("paginate_button");
            nextButton.AddCssClass("next");
            var aNextButton = new TagBuilder("a");
            aNextButton.InnerHtml.SetContent("Previous");
            if (page < totalPages)
            {
                aNextButton.Attributes["href"] = buildUrlForPage(page - 1);
            }
            else
            {
                nextButton.AddCssClass("disabled");
            }

            nextButton.InnerHtml.SetHtmlContent(aNextButton);
            rootUlElement.InnerHtml.AppendHtml(nextButton);


            return new HtmlString(rootUlElement.ToString());
        }
    }
}