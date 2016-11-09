using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class ExistsUserCommand : GetCommand<bool>
    {
        public string Id { get; private set; }

        public ExistsUserCommand Setup( string id)
        {
            this.Id = id;
            return this;
        }

        public override Task<bool> GetAsync()
        {
            return this.Context.Users.AnyAsync(us => us.Id == this.Id);

        }
    }
}