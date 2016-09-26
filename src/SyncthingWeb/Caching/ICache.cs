using System;
using System.Threading.Tasks;

namespace SyncthingWeb.Caching
{
    public interface ICache
    {
        void Put<T>(string key, T item);

        T Get<T>(string key);

        T Get<T>(string key, Func<CacheContext, T> addItemFactory);

        Task<T> GetAsync<T>(string key, Func<CacheContext, Task<T>> addItemFactory);

        T Get<T>(string key, Func<CacheContext, T> addItemFactory, TimeSpan slidingExpiration);

        Task<T> GetAsync<T>(string key, Func<CacheContext, Task<T>> addItemFactory, TimeSpan slidingExpiration);

        void Signal(string key);
    }

    public class CacheContext
    {
        public string Key { get; private set; }

        public CacheContext(string key)
        {
            this.Key = key;
        }
    }
}
