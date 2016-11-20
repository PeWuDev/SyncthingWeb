using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SyncthingWeb.Extensions;

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
            this.Context.AddOrUpdate(this.Entity);
            return this.Context.SaveChangesAsync();
        }
    }
}