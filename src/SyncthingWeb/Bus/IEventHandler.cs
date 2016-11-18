using System.Threading.Tasks;

namespace SyncthingWeb.Bus
{
    public interface IEventHandler<TEvent> 
    {
        Task HandleAsync(TEvent @event);
    }
}
