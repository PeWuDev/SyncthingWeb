using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.SharedEntries
{
    public class GetSharedEntriesCountCommand : GetCommand<int>
    {
        public string User { get; private set; }

        public GetSharedEntriesCountCommand Setup(string user)
        {
            this.User = user;
            return this;
        }

        public override Task<int> GetAsync()
        {
            return this.Context.SharedEntries.Where(s => s.OwnerId == this.User).CountAsync();
        }
    }
}