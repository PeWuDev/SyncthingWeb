using System.Threading.Tasks;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class GetAllowedFolderCommand : GetCommand<AllowedFolder>
    {
        public string User { get; private set; }
        public string Folder { get; private set; }

        public GetAllowedFolderCommand Setup(string user, string folder)
        {
            this.User = user;
            this.Folder = folder;
            return this;
        }

        public override async Task<AllowedFolder> GetAsync()
        {
            var allFolders = await this.CommandFactory.Create<GetAllowedFoldersCommand>().Setup(this.User).GetAsync();
            if (!allFolders.ContainsKey(this.Folder)) return null;

            return allFolders[this.Folder];
        }
    }
}