using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Commands.Implementation.SharedEntries
{
    public class StopSharingCommand : NonQueryCommand
    {
        public int Id { get; private set; }


        public StopSharingCommand Setup(int id)
        {
            this.Id = id;
            return this;
        }

        public override async Task ExecuteAsync()
        {
            foreach (var item in this.Context.SharedEntries.Where(sh => sh.Id == this.Id))
            {
                this.Context.SharedEntries.Remove(item);
            }

            await this.Context.SaveChangesAsync();
        }
    }
}