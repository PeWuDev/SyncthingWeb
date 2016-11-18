using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class ExistsFolderCommand: GetCommand<bool>
    {
        public string Folder { get; private set; }

        public ExistsFolderCommand Setup(string folderId)
        {
            this.Folder = folderId;
            return this;
        }

        public override Task<bool> GetAsync()
        {
            return
                this.CommandFactory.Create<QueryAllFoldersCommand>()
                    .ExecuteAsync().ContinueWith(t => t.Result.Any(fld => fld.FolderId == this.Folder));
        }
    }
}