using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SyncthingWeb.Bus;

namespace SyncthingWeb.Permissions
{
    internal abstract class PermissionProviderBase : IEventHandler<IPermissionCollector>
    {

        public Task HandleAsync(IPermissionCollector @event)
        {

            var type = this.GetType();
            var permFields =
                type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(t => t.FieldType == typeof(Permission));

            foreach (var value in permFields.Select(permField => permField.GetValue(this)).Cast<Permission>())
            {
                @event.Add(value);
            }

            return Task.FromResult(0);
        }
    }
}
