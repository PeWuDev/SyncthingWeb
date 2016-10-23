using Autofac;
using SyncthingWeb.Notifications;

namespace SyncthingWeb.Modules
{
    public class NotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultNotification>().AsImplementedInterfaces().PropertiesAutowired();
        }
    }
}
