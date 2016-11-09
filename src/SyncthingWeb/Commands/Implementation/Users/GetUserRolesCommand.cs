using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Caching;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class GetUserRolesCommand : GetCommand<IEnumerable<IdentityUserRole<string>>>
    {
        private readonly ICache cache;

        public GetUserRolesCommand(ICache cache)
        {
            this.cache = cache;
        }

        public string UserId { get; private set; }

        public GetUserRolesCommand Setup(string userId)
        {
            this.UserId = userId;
            return this;
        }

        public override Task<IEnumerable<IdentityUserRole<string>>> GetAsync()
        {
            return this.cache.GetAsync("user-roles-" + this.UserId,
                ctx => this.Context.Users.AsNoTracking().SingleAsync(u => u.Id == this.UserId)
                    .ContinueWith(usrTask => usrTask.Result.Roles.AsEnumerable()));
        }
    }
}