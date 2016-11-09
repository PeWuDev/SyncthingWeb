using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class RoleExistsCommand : GetCommand<bool>
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public RoleExistsCommand SetupById(string id)
        {
            this.Id = id;
            return this;
        }

        public RoleExistsCommand SetupByName(string name)
        {
            this.Name = name;
            return this;
        }

        public override Task<bool> GetAsync()
        {
            var queryable = this.Context.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(this.Id))
            {
                queryable = queryable.Where(ir => ir.Id == this.Id);
            }

            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                queryable = queryable.Where(ir => ir.Name == this.Name);
            }

            return queryable.AnyAsync();
        }
    }
}