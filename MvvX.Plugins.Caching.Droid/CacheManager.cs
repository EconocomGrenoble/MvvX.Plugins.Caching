using MvvX.Plugins.Caching.Droid.System.Runtime.Caching;
using System;
using System.Collections.Generic;

namespace MvvX.Plugins.Caching.Droid
{
    /// <summary>
    /// Represents a MemoryCacheCache
    /// </summary>
    public class CacheManager : ICacheManager
    {
        #region Constructor

        private readonly string globalDependencyKey = "MvvX.Plugins.Caching.GlobalCacheKey";

        public CacheManager(string globalDependencyKey)
        {
            this.globalDependencyKey = globalDependencyKey;
        }

        #endregion

        #region cache keys lists

        private CustomMemoryCache Cache
        {
            get
            {
                return CustomMemoryCache.Default;
            }
        }

        private CacheEntryChangeMonitor GetCacheDependency(string dependantKey)
        {
            if (Cache[globalDependencyKey] == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.MaxValue
                };
                Cache.Add(globalDependencyKey, DateTime.Now.ToString(), policy);
            }

            if (dependantKey != globalDependencyKey && Cache[dependantKey.ToLower()] == null)
            {
                CacheItem item = new CacheItem(dependantKey.ToLower(), DateTime.Now.ToString());
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
                policy.ChangeMonitors.Add(GetCacheDependency(globalDependencyKey));
                Cache.Add(item, policy);

                List<string> keys = new List<string>() { dependantKey.ToLower() };
                return Cache.CreateCacheEntryChangeMonitor(keys);
            }
            else
            {
                List<string> keys = new List<string>() { globalDependencyKey };
                return Cache.CreateCacheEntryChangeMonitor(keys);
            }
        }

        #endregion

        public T Get<T>(string key)
        {
            if (IsSet(key))
            {
                return (T)Cache[key.ToLower()];
            }
            else
            {
                return default(T);
            }
        }

        public void Set(string key, object data, int cacheTime, string patternKey)
        {
            if (data == null || string.IsNullOrWhiteSpace(key))
                return;

            string lowerCacheKey = key.ToLower();
            int duration = GetRandomDuration(cacheTime);

            CacheItem item = new CacheItem(lowerCacheKey, data);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(duration);
            policy.ChangeMonitors.Add(GetCacheDependency(patternKey));
            Cache.Add(item, policy);
        }

        private int GetRandomDuration(int duration)
        {
            int minDuration = duration * (100 - 25) / 100;
            int maxDuration = duration * (100 + 25) / 100;
            return new Random().Next(minDuration, maxDuration);
        }

        /// <summary>
        /// Indique si une élément existe dans le cache pour la clée donnée
        /// </summary>
        /// <param name="key">Clée à tester</param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            return !IsThisNull(key);
        }

        /// <summary>
        /// Tente de récupérer un objet dans le cache depuis sa clée
        /// </summary>
        /// <param name="cacheKey">Clée à tester</param>
        /// <returns></returns>
        public object RetrieveThis(string cacheKey)
        {
            return Cache[cacheKey.ToLower()];
        }

        /// <summary>
        /// Indique si dans le cache l'élément défini par sa clée est null
        /// </summary>
        /// <param name="cacheKey">Clée à tester</param>
        /// <returns>Vrai si l'objet est null ou non défini</returns>
        public bool IsThisNull(string cacheKey)
        {
            return (string.IsNullOrWhiteSpace(cacheKey) || Cache[cacheKey.ToLower()] == null);
        }

        public void Remove(string cacheKey)
        {
            Cache.Remove(cacheKey.ToLower());
        }

        public void Clear()
        {
            Cache.Remove(globalDependencyKey);
        }

        /// <summary>
        /// Renvoie la valeur en cache définie par sa clé.
        /// Si l'objet n'existe pas, il est généré puis mis en cache avant d'être renvoyé
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="cacheKey">clé</param>
        /// <param name="patternKey">Clé parente</param>
        /// <param name="acquire">Fonction d'acquisition de l'objet</param>
        /// <returns>Objet dans le cache</returns>
        public T Get<T>(string cacheKey, string patternKey, Func<T> acquire)
        {
            return Get(cacheKey, 60, patternKey, acquire);
        }

        /// <summary>
        /// Renvoie la valeur en cache définie par sa clé.
        /// Si l'objet n'existe pas, il est généré puis mis en cache avant d'être renvoyé
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="cacheKey">clé</param>
        /// <param name="cacheTime">Durée de stockage dans le cache</param>
        /// <param name="patternKey">Clé parente</param>
        /// <param name="acquire">Fonction d'acquisition de l'objet</param>
        /// <returns>Objet dans le cache</returns>
        public T Get<T>(string cacheKey, int cacheTime, string patternKey, Func<T> acquire)
        {
            if (IsSet(cacheKey))
            {
                return Get<T>(cacheKey);
            }
            else
            {
                var result = acquire();
                Set(cacheKey, result, cacheTime, patternKey);
                return result;
            }
        }
    }
}
