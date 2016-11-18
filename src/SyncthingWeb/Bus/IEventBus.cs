using System.Threading.Tasks;

namespace SyncthingWeb.Bus
{
    public interface IEventBus
    {
        Task Trigger<TEvent>(TEvent @event);
    }
}