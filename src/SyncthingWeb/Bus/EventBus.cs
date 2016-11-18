using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace SyncthingWeb.Bus
{
    public class EventBus : IEventBus
    {
        private readonly IComponentContext componentContext;

        public EventBus(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public Task Trigger<TEvent>(TEvent @event)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));
            var handlers =
                (IEnumerable<IEventHandler<TEvent>>)
                componentContext.Resolve(typeof(IEnumerable<>).MakeGenericType(handlerType));


            return Task.WhenAll(handlers.Select(hn => hn.HandleAsync(@event)));
        }
    }
}
