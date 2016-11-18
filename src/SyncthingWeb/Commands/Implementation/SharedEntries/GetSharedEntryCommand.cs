using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.SharedEntries
{
    public class GetSharedEntryCommand : GetCommand<SharedEntry>
    {
        public int Id { get; private set; }

        public GetSharedEntryCommand Setup(int id)
        {
            this.Id = id;
            return this;
        }

        public override Task<SharedEntry> GetAsync()
        {
            return this.Context.SharedEntries.AsNoTracking().Include(sh => sh.Folder).SingleOrDefaultAsync(sh => sh.Id == this.Id);
        }
    }
}