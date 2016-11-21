using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;
using System.Linq;
using Autofac;
using Syncthing.Integration;

namespace SyncthingWeb.Syncthing
{
    public class DefaultSyncthingContextFactory : ISyncthingContextFactory
    {
        private const string ContextCacheKey = "syncthing-current-context";

        private static object WatchingLock = new object();
        private static bool Watching = false;

        private readonly ICache cache;

        private readonly ICommandFactory commandsFactory;

        public DefaultSyncthingContextFactory(ICache cache, ICommandFactory commandsFactory)
        {
            this.cache = cache;
            this.commandsFactory = commandsFactory;
        }

        public Task<SyncthingContext> GetContext()
        {
            return this.cache.GetAsync(
                ContextCacheKey,
                async context =>
                {
                    var generalSettings =
                        await this.commandsFactory.Create<GetCurrentGeneralSettingsCommand>().SetupNoCache(true).GetAsync();
                    Watch(generalSettings);
                    var ctx = SyncthingContext.Create(generalSettings.SyncthingConfigPath);
                    await this.SynchronizeDatabase(ctx);
                    return ctx;
                });
        }

        private async Task SynchronizeDatabase(SyncthingContext ctx)
        {
            var allEnumerated = await this.commandsFactory.Create<QueryAllFoldersCommand>().NoCache().ExecuteAsync(); ;
            var all = allEnumerated as IList<Folder> ?? allEnumerated.ToList();

            var newFolders = ctx.Folders.Select(f => f.Id).Except(all.Select(f => f.FolderId));
            await this.commandsFactory.Create<EnsureNewFoldersCommand>().Setup(newFolders.ToArray()).ExecuteAsync();

            var toRemove = all.Select(f => f.FolderId).Except(ctx.Folders.Select(f => f.Id));
            await this.commandsFactory.Create<RemoveFolderCommand>().Setup(toRemove.ToArray()).ExecuteAsync();
        }

        private static void Watch(GeneralSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SyncthingConfigPath)) return;
            if (Watching) return;

            lock (WatchingLock)
            {
                if (Watching) return;

                var fw = new FileSystemWatcher
                {
                    Path = Path.GetDirectoryName(settings.SyncthingConfigPath),
                    Filter = Path.GetFileName(settings.SyncthingConfigPath)
                };

                fw.Changed += (sender, args) =>
                {
                    //TODO other access
                    //direct access to prevent no httpcontext errors
                    Startup.ApplicationContainer.Resolve<ICache>().Signal(ContextCacheKey);
                };

                fw.EnableRaisingEvents = true;

                Watching = true;
            }
        }


        public void Reload()
        {
            this.cache.Signal(ContextCacheKey);
        }
    }
}
