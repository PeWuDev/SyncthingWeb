using Autofac;
using SyncthingWeb.Permissions;
using SyncthingWebUI.Authorization;

namespace SyncthingWeb.Authorization
{
    public class AuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultAuthorizer>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultPermissionResolver>().As<IPermissionResolver>().InstancePerLifetimeScope();
        }
    }
}