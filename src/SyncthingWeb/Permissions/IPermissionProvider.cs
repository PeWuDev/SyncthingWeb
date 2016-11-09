namespace SyncthingWebUI.Permissions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal interface IPermissionProvider /*: IEvent */
    {
        void Register(ICollection<Permission> permissions);
    }

    internal abstract class PermissionProviderBase : IPermissionProvider
    {
        public virtual void Register(ICollection<Permission> permissions)
        {
            var type = this.GetType();
            var permFields =
                type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(t => t.FieldType == typeof(Permission));

            foreach (var value in permFields.Select(permField => permField.GetValue(this)).Cast<Permission>())
            {
                permissions.Add(value);
            }
        }
    }
}
