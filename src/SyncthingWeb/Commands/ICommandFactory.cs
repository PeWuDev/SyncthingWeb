namespace SyncthingWeb.Commands
{
    public interface ICommandFactory
    {
        TCommand Create<TCommand>() where TCommand : CommandBase;
    }
}
