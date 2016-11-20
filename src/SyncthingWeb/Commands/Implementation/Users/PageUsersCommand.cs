using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Models;
using SyncthingWebUI.Helpers;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class PageUsersCommand : PagedQueryCommand<ApplicationUser, GetUsersQuery>
    {
        public PageUsersCommand Setup(GetUsersQuery query)
        {
            this.Query = query;
            return this;
        }

        protected override  Task<IOrderedQueryable<ApplicationUser>> GetQueryable()
        {
            var q = this.Context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(this.Query.Name)) q = q.Where(u => u.UserName.Contains(this.Query.Name));

            if (this.Query.ExcludedRoles != null && this.Query.ExcludedRoles.Any())
            {
                q = q.Where(u => !u.Roles.Any(r => this.Query.ExcludedRoles.Contains(r.RoleId)));
            }

            return Task.FromResult(q.OrderBy(u => u.Id));
        }
    }

    public class GetUsersQuery : PagedQuery
    {
        public string Name { get; set; }
        public string[] ExcludedRoles { get; set; }
    }

  
}