using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class QueryUsersIdInRolesCommand : QueryCommand<string>
    {
        public string[] Roles { get; private set; }

        public QueryUsersIdInRolesCommand Setup(params string[] roles)
        {
            this.Roles = roles;
            return this;
        }

        public override async Task<IEnumerable<string>> ExecuteAsync()
        {
            return await
                this.Context.Roles.AsNoTracking()
                    .Where(r => this.Roles.Contains(r.Id))
                    .SelectMany(r => r.Users.Select(usr => usr.UserId))
                    .ToListAsync();
        }
    }
}