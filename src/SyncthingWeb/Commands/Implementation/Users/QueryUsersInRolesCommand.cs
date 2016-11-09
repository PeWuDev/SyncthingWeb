using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class QueryUsersInRolesCommand : QueryCommand<ApplicationUser>
    {
        public string[] Roles { get; private set; }

        public QueryUsersInRolesCommand Setup(params string[] roles)
        {
            this.Roles = roles;
            return this;
        }
        public override async Task<IEnumerable<ApplicationUser>> ExecuteAsync()
        {
            return await
                this.Context.Users.AsNoTracking()
                    .Where(usr => usr.Roles.Any(rl => this.Roles.Contains(rl.RoleId)))
                    .ToListAsync();
        }
    }
}