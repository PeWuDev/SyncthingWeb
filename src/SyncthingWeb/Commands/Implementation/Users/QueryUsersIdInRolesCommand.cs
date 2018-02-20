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

        public override Task<IEnumerable<string>> ExecuteAsync()
        {
            return (from userRole in Context.UserRoles.AsNoTracking()
                    where Roles.Contains(userRole.RoleId)
                    select userRole.UserId).Distinct().ToListAsync()
                .ContinueWith(queryTaskResult => queryTaskResult.Result.AsEnumerable());

        }
    }
}