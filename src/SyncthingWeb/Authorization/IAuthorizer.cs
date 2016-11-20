using System.Threading.Tasks;
using SyncthingWeb.Permissions;

namespace SyncthingWeb.Authorization
{
    public interface IAuthorizer
    {
        Task<bool> IsSuperAdminAsync(string userId = null);
        Task<bool> AuthorizeAsync(Permission permission, string userId = null);
    }
}
