using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SyncthingWeb.Caching;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Folders;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Models;
using System.Linq;
using System.Threading;
using Autofac;
using Syncthing.Integration;
using Syncthing.Integration.Configuration;

namespace SyncthingWeb.Syncthing
{
    public class DefaultSyncthingContextFactory : ISyncthingContextFactory
    {
        private const string ContextCacheKey = "syncthing-current-context";

        private static object WatchingLock = new object();
        private static Thread watchingThread = null;

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
                        await
                            this.commandsFactory.Create<GetCurrentGeneralSettingsCommand>()
                                .SetupNoCache(true)
                                .GetAsync();

                    var ctx =
                        await
                            SyncthingContext.CreateAsync(new SyncthingApiEndpoint(generalSettings.SyncthingApiKey,
                                generalSettings.SyncthingEndpoint));

                    await this.SynchronizeDatabase(ctx);

                    Watch(ctx);

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

        private static void Watch(SyncthingContext ctx)
        {
            //lock (WatchingLock)
            //{
            //    if (watchingThread != null && watchingThread.ThreadState != ThreadState.Stopped)
            //        throw new InvalidOperationException("Last watching thread ended unexpected");

            //    watchingThread = new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            try
            //            {
            //                Thread.Sleep(TimeSpan.FromMinutes(5));
            //            } catch () 

            //            var changed = Task.Run(async () => await ctx.ConfigWatcher.ChangedAsync()).Result;
            //            if (!changed)
            //            {
            //                continue;
            //            }

            //            var cache = Startup.ApplicationContainer.Resolve<ICache>();
            //            cache.Signal(ContextCacheKey);

            //            break;
            //        }
            //    });

            //    watchingThread.Start();
            //}
        }


        public void Reload()
        {
            this.cache.Signal(ContextCacheKey);
        }
    }
}
