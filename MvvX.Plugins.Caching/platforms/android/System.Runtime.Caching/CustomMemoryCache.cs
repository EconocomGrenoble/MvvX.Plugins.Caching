using System.Collections.Generic;
using System.Linq;

namespace MvvX.Plugins.Caching.System.Runtime.Caching
{
    internal class CustomMemoryCache
    {
        private static CustomMemoryCache @default;
        public static CustomMemoryCache Default {
            get
            {
                if (@default == null)
                    @default = new CustomMemoryCache();
                return @default;
            }
        }

        public object this[string cacheKey]
        {
            get
            {
                if (cache.ContainsKey(cacheKey))
                    return cache[cacheKey].Value;
                else
                    return null;
            }
        }

        private readonly IDictionary<string, CacheItem> cache;
        /// <summary>
        /// Contains, for a specific key, all dependant keys.
        /// </summary>
        private readonly IDictionary<string, List<string>> dependencyKeys;

        public CustomMemoryCache()
        {
            cache = new Dictionary<string, CacheItem>();
            dependencyKeys = new Dictionary<string, List<string>>();
        }

        internal CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(List<string> keys)
        {
            return new CacheEntryChangeMonitor()
            {
                DependencyKeys = keys
            };
        }

        internal void Add(string cacheKey, string value, CacheItemPolicy policy)
        {
            Add(new CacheItem(cacheKey, value), policy);
        }

        internal void Add(CacheItem item, CacheItemPolicy policy)
        {
            Remove(item.CacheKey);

            // We will add the item :
            cache.Add(item.CacheKey, item);
            
            // Check policy :
            foreach (var changeMonitor in policy.ChangeMonitors)
            {
                foreach (var dependencyKey in changeMonitor.DependencyKeys)
                {
                    if (dependencyKeys.ContainsKey(dependencyKey))
                    {
                        var keys = dependencyKeys[dependencyKey];
                        if (!keys.Any(e => e == item.CacheKey))
                            keys.Add(item.CacheKey);
                    }
                    else
                    {
                        dependencyKeys.Add(dependencyKey, new List<string>() { item.CacheKey });
                    }
                }
            }
        }

        internal void Remove(string cacheKey)
        {
            // Remove item in cache
            if (cache.ContainsKey(cacheKey))
                cache.Remove(cacheKey);

            // Remove child dependencies
            if (dependencyKeys.ContainsKey(cacheKey.ToLower()))
            {
                var currentDependencyKeys = dependencyKeys[cacheKey.ToLower()];
                if(currentDependencyKeys.Any())
                {
                    foreach (var key in currentDependencyKeys)
                        Remove(key);
                }
                dependencyKeys.Remove(cacheKey.ToLower());
            }
        }
    }
}