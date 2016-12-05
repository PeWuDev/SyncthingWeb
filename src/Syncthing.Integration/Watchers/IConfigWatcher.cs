using System.Threading.Tasks;

namespace Syncthing.Integration.Watchers
{
    public interface IConfigWatcher
    {
        Task<bool> ChangedAsync();
    }
}
