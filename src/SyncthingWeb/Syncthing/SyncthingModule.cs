using Autofac;

namespace SyncthingWeb.Syncthing
{
    public class SyncthingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultSyncthingContextFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
