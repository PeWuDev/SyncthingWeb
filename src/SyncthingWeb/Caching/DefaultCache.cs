using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace SyncthingWeb.Caching
{
    public class DefaultCache : ICache
    {
        private readonly IMemoryCache appCache;
        private readonly SemaphoreSlim asyncSemaphore;

        private readonly Dictionary<string, SemaphoreSlim> localSemaphores;

        public DefaultCache(IMemoryCache appCache)
        {
            this.appCache = appCache;

            this.asyncSemaphore = new SemaphoreSlim(1);
            this.localSemaphores = new Dictionary<string, SemaphoreSlim>();
        }

        public void Put<T>(string key, T item)
        {
            this.appCache.Set(key, item);
        }

        public T Get<T>(string key)
        {
            return this.appCache.Get<T>(key);
        }

        public T Get<T>(string key, Func<CacheContext, T> addItemFactory)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            }

            if (addItemFactory == null)
            {
                throw new ArgumentNullException(nameof(addItemFactory), "addItemFactory factory cannot be null.");
            }

            return this.appCache.GetOrCreate(key, c => addItemFactory(new CacheContext(key)));
        }

        public Task<T> GetAsync<T>(string key, Func<CacheContext, Task<T>> addItemFactory)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            }

            if (addItemFactory == null)
            {
                throw new ArgumentNullException(nameof(addItemFactory), "addItemFactory factory cannot be null.");
            }

            return this.GetAsync(key, addItemFactory, TimeSpan.MaxValue);
        }

        public T Get<T>(string key, Func<CacheContext, T> addItemFactory, TimeSpan slidingExpiration)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
            }

            if (addItemFactory == null)
            {
                throw new ArgumentNullException(nameof(addItemFactory), "addItemFactory factory cannot be null.");
            }

            return this.appCache.GetOrCreate(key, c =>
            {
                c.SetSlidingExpiration(slidingExpiration);
                return addItemFactory(new CacheContext(key));
            });
        }

        public async Task<T> GetAsync<T>(string key, Func<CacheContext, Task<T>> addItemFactory, TimeSpan slidingExpiration)
        {
            var result = this.Get<T>(key);
            if (result != null && !result.Equals(default(T)))
            {
                return result;
            }

            if (!this.localSemaphores.ContainsKey(key))
            {
                await this.asyncSemaphore.WaitAsync();

                if (!this.localSemaphores.ContainsKey(key))
                {
                    this.localSemaphores.Add(key, new SemaphoreSlim(1));
                }

                this.asyncSemaphore.Release();
            }

            var sem = this.localSemaphores[key];
            await sem.WaitAsync();

            try
            {
                result = this.Get<T>(key);

                if (result == null || result.Equals(default(T)))
                {
                    result = await addItemFactory(new CacheContext(key));
                    this.appCache.Set(key, result, TimeSpan.FromDays(1));
                }
            }
            finally
            {
                sem.Release();
            }

            return result;
        }

        public void Signal(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot null or emprty.", nameof(key));
            }

            this.appCache.Remove(key);

            if (this.localSemaphores.ContainsKey(key))
            {
                var sem = this.localSemaphores[key];
                sem.Wait();

                if (this.localSemaphores.ContainsKey(key))
                    this.localSemaphores.Remove(key);

                sem.Release();
            }
        }


     
    }
}
