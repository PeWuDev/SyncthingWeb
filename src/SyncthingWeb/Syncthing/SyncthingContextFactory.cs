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
using FluentScheduler;
using Microsoft.Extensions.Logging;
using Syncthing.Integration;
using Syncthing.Integration.Configuration;

namespace SyncthingWeb.Syncthing
{
    public class DefaultSyncthingContextFactory : ISyncthingContextFactory
    {
        private const string ContextCacheKey = "syncthing-current-context";

        private static readonly object WatchingLock = new object();

        private readonly ICache cache;

        private readonly ICommandFactory commandsFactory;

        private readonly ILoggerFactory loggerFactory;

        public DefaultSyncthingContextFactory(ICache cache, ICommandFactory commandsFactory, ILoggerFactory loggerFactory)
        {
            this.cache = cache;
            this.commandsFactory = commandsFactory;
            this.loggerFactory = loggerFactory;
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

                    var logger = loggerFactory.CreateLogger(typeof(SyncthingContext));
                    ctx.Configuration.SetErrorCallback(message => logger.LogError(message));

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
            const string jobName = "SycnthingContextWatch";

            lock (WatchingLock)
            {
                JobManager.RemoveJob(jobName);
                JobManager.AddJob(async () =>
                {
                    var changed = await ctx.ConfigWatcher.ChangedAsync();
                    if (!changed) return;

                    var cache = Startup.ApplicationContainer.Resolve<ICache>();
                    cache.Signal(ContextCacheKey);
                }, schedule => schedule.WithName(jobName).ToRunEvery(5).Minutes());
            }
        }


        public void Reload()
        {
            this.cache.Signal(ContextCacheKey);
        }
    }
}
