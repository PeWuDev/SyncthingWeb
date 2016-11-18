using System.Reflection;
using Autofac;

namespace SyncthingWeb.Commands
{
    using Module = Autofac.Module;

    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var dataAccess = Assembly.GetEntryAssembly();

            builder.RegisterType<DefaultTraManager>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder.RegisterType<DefaultCommandFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(
                    t =>
                        typeof(CommandBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract &&
                        t.GetTypeInfo().IsClass)
                .InstancePerDependency()
                .PropertiesAutowired();
        }
    }
}