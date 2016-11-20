using SyncthingWeb.Areas.Folders.Services;

namespace SyncthingWebUI.Areas.Folders.Module
{
    using Autofac;

    public class FoldersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultSyncthingFIleFetcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}