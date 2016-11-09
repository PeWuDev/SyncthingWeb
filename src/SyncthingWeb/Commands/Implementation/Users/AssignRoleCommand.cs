using System.Linq;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Commands.Implementation.Events;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class DeassignRoleCommand : NonQueryCommand
    {
        private readonly ICache cache;

        private readonly IUserRolesChanged changedEvent;

        public DeassignRoleCommand(ICache cache, IUserRolesChanged changedEvent)
        {
            this.cache = cache;
            this.changedEvent = changedEvent;
        }

        public string Role { get; private set; }
        public string User { get; private set; }

        public DeassignRoleCommand Setup(string role, string user)
        {
            this.Role = role;
            this.User = user;

            return this;
        }

        public override async Task ExecuteAsync()
        {
            var user = await this.Context.Users.SingleAsync(u => u.Id == this.User);
            if (user.Roles.Any(ru => ru.RoleId == this.Role))
            {
                try
                {
                    var ru = user.Roles.Single(role => role.RoleId == this.Role);
                    user.Roles.Remove(ru);

                    await this.Context.SaveChangesAsync();
                    await this.changedEvent.Remove(this.User);
                }
                finally
                {
                    this.cache.Signal("user-roles-" + this.User);
                }
            }
        }
    }
}