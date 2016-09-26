using Autofac;

namespace SyncthingWeb.Commands
{
    public class DefaultCommandFactory : ICommandFactory
    {
        private readonly ILifetimeScope scope;

        public DefaultCommandFactory(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public TCommand Create<TCommand>() where TCommand : CommandBase
        {
            return this.scope.Resolve<TCommand>();
        }
    }
}