using System.Threading.Tasks;

namespace SyncthingWeb.Commands.Implementation.Events
{
    public interface IUserRolesChanged /*: IEvent*/
    {
        Task Added(string userId);

        Task Remove(string userId);
    }
}
