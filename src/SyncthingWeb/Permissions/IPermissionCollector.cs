using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SyncthingWeb.Permissions
{
    public class PermissionCollector : IPermissionCollector
    {
        private readonly IList<Permission> p = new List<Permission>();

        public IEnumerable<Permission> Permissions => new ReadOnlyCollection<Permission>(p);
        public void Add(Permission permission)
        {
            this.p.Add(permission);
        }
    }
    public interface IPermissionCollector
    {
        void Add(Permission permission);
    }
}
