using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Bus;
using SyncthingWeb.Helpers;
using SyncthingWeb.Models;
using SyncthingWeb.Searching;

namespace SyncthingWeb.Controllers
{
    [Authorize]
    public class SearchController : ExtendedController
    {
        private readonly IEventBus eventBus;
        private readonly UserManager<ApplicationUser> userManager;

        public SearchController(UserManager<ApplicationUser> userManager, IEventBus eventBus)
        {
            this.userManager = userManager;
            this.eventBus = eventBus;
        }

        public Task<ActionResult> Index(string id)
        {
            return Task.FromResult((ActionResult) this.View((object) id));
        }

        public async Task<ActionResult> IndexApi(string id)
        {
            if (id == null || id.Length <= 2) return new EmptyResult();

            var result = new List<SearchResultItem>();
            var userId = this.userManager.GetUserId(this.User);

            var searchColl = new SearchCollector(id, userId);
            await eventBus.Trigger<ISearchCollector>(searchColl);

            return this.PartialView(searchColl.Items);
        }
    }
}