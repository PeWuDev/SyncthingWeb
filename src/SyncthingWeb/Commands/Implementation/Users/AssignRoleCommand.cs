using System;
using System.Linq;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Bus;
using SyncthingWeb.Commands.Implementation.Events;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class DeassignRoleCommand : NonQueryCommand
    {
        private readonly ICache cache;
        private readonly IEventBus eventBus;

        public DeassignRoleCommand(ICache cache, IEventBus eventBus)
        {
            if (eventBus == null) throw new ArgumentNullException(nameof(eventBus));
            this.cache = cache;
            this.eventBus = eventBus;
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
                    await this.eventBus.Trigger(new RemovedUserRoleEvent(this.User));
                }
                finally
                {
                    this.cache.Signal("user-roles-" + this.User);
                }
            }
        }
    }
}