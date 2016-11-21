using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncthingWeb.Permissions
{
    public interface IPermissionResolver
    {
        Task<HashSet<Permission>> GetForUser(string userId);
        Task<HashSet<Permission>> GetForRole(string id);
        Task SetForRole(string id, IEnumerable<string> perms);


        void UserPermissionsChanged(string userId);

        Task<bool> Authorize(Permission permission, string userId);
        IEnumerable<Permission> All { get; }
    }
}