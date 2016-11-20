using Autofac;

namespace SyncthingWeb.Bus
{
    public class EventBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();
        }
    }
}
