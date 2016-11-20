using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SyncthingWebUI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class PagedRolesCommand : PagedQueryCommand<IdentityRole, RoleQuery>
    {
        public PagedRolesCommand Setup(RoleQuery query)
        {
            this.Query = query;
            return this;
        }
        protected override Task<IOrderedQueryable<IdentityRole>> GetQueryable()
        {
            var q = this.Context.Roles.AsNoTracking();

            return Task.FromResult(q.OrderBy(r => r.Id));
        }
    }

    public class RoleQuery : PagedQuery
    {
        public string Name { get; set; }
    }
}