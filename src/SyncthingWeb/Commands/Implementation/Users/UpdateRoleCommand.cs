using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class UpdateRoleCommand : NonQueryCommand
    {
        public IdentityRole Entity { get; private set; }

        public UpdateRoleCommand Setup(IdentityRole entity)
        {
            this.Entity = entity;
            return this;
        }

        public override Task ExecuteAsync()
        {
            if (this.Entity.Id == null) this.Context.Roles.Add(this.Entity);
            else this.Context.Roles.Update(this.Entity);
            return this.Context.SaveChangesAsync();
        }
    }
}