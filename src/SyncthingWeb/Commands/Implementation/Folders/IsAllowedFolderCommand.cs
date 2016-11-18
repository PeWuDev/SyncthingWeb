using System.Collections.Generic;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Folders
{
    public class IsAllowedFolderCommand : GetCommand<bool>
    {
        private readonly ICache cache;

        public IsAllowedFolderCommand(ICache cache)
        {
            this.cache = cache;
        }

        public string User { get; private set; }
        public string Folder { get; private set; }


        public IsAllowedFolderCommand Setup(string user, string folder)
        {
            this.User = user;
            this.Folder = folder;
            return this;
        }

        public override async Task<bool> GetAsync()
        {
            var cfg = await this.CommandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync();
            if (cfg.AdminId == this.User) return true;

            var allowed = await this.AllowedAsync(this.User);
            return allowed.ContainsKey(this.Folder);
        }

        private Task<Dictionary<string, AllowedFolder>> AllowedAsync(string user)
        {
            return this.CommandFactory.Create<GetAllowedFoldersCommand>().Setup(user).GetAsync();
        }
    }
}