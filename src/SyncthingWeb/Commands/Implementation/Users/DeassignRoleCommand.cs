using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SyncthingWeb.Caching;
using SyncthingWeb.Commands.Implementation.Events;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class AssignRoleCommand : NonQueryCommand
    {
        private readonly ICache cache;

        private readonly IUserRolesChanged changedEvent;

        public AssignRoleCommand(ICache cache, IUserRolesChanged changedEvent)
        {
            this.cache = cache;
            this.changedEvent = changedEvent;
        }

        public string Role { get; private set; }
        public string User { get; private set; }

        public AssignRoleCommand Setup(string role, string user)
        {
            this.Role = role;
            this.User = user;

            return this;
        }

        public override async Task ExecuteAsync()
        {
            var user = await this.Context.Users.SingleAsync(u => u.Id == this.User);
            if (user.Roles.All(ru => ru.RoleId != this.Role))
            {
                try
                {
                    user.Roles.Add(new IdentityUserRole<string> { RoleId = this.Role });
                    await this.Context.SaveChangesAsync();

                    await this.changedEvent.Added(this.User);
                }
                finally
                {
                    this.cache.Signal("user-roles-" + this.User);
                }
            }
        }
    }
}