using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class GetRoleCommand : GetCommand<IdentityRole>
    {
        public string Id { get; private set; }

        public GetRoleCommand Setup(string id)
        {
            this.Id = id;
            return this;
        }

        public override Task<IdentityRole> GetAsync()
        {
            return this.Context.Roles.AsNoTracking().SingleOrDefaultAsync(ir => ir.Id == this.Id);
        }
    }
}