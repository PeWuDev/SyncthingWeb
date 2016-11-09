using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class QueryRolesPermissionsNamesCommand :QueryCommand<string>
    {
        public string[] Roles { get; private set; }

        public QueryRolesPermissionsNamesCommand Setup(params string[] roles)
        {
            this.Roles = roles;
            return this;
        }

        public override async Task<IEnumerable<string>> ExecuteAsync()
        {
            return
                await
                    this.Context.RolePermissions.AsNoTracking().Where(rp => this.Roles.Contains(rp.RoleId))
                        .Select(rp => rp.Name)
                        .Distinct()
                        .ToListAsync();
        }
    }
}