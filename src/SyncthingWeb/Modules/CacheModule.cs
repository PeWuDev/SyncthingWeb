using Autofac;
using SyncthingWeb.Caching;

namespace SyncthingWeb.Modules
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DefaultCache>().As<ICache>().SingleInstance();
        }
    }
}