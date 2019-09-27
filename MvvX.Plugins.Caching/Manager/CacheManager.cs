using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MvvX.Plugins.Caching
{
    /// <summary>
    /// Represents a MemoryCacheCache
    /// </summary>
    public class CacheManager : ICacheManager
    {
        #region Constructor

        private readonly string globalDependencyKey;

        public CacheManager()
        {
            this.globalDependencyKey = "MvvX.Plugins.Caching.GlobalCacheKey";
        }

        public CacheManager(string globalDependencyKey)
        {
            this.globalDependencyKey = globalDependencyKey;
        }

        #endregion

        #region Cache keys lists

        private ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        private CacheEntryChangeMonitor GetCacheDependency(string dependantKey)
        {
            if (Cache[globalDependencyKey] == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
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
        
        public T Get<T>(string cacheKey)
        {
            if (IsSet(cacheKey))
            {
                return (T)Cache[cacheKey.ToLower()];
            }
            else
            {
                return default(T);
            }
        }

        public void Set(string key, object data, int cacheTime, string patternKey)
        {
            if (data == null)
                return;

            if (string.IsNullOrWhiteSpace(key))
                return;

            string lowerCacheKey = key.ToLower();

            CacheItem item = new CacheItem(lowerCacheKey, data);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTime);
            policy.ChangeMonitors.Add(GetCacheDependency(patternKey));
            Cache.Add(item, policy);

            List<string> keys = new List<string>() { patternKey.ToLower() };
            Cache.CreateCacheEntryChangeMonitor(keys);
        }

        /// <summary>
        /// Indique si une élément existe dans le cache pour la clée donnée
        /// </summary>
        /// <param name="cacheKey">Clée à tester</param>
        /// <returns></returns>
        public bool IsSet(string cacheKey)
        {
            return !IsThisNull(cacheKey);
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
        /// <param name="key">clé</param>
        /// <param name="cacheTime">Durée de stockage dans le cache</param>
        /// <param name="patternKey">Clé parente</param>
        /// <param name="acquire">Fonction d'acquisition de l'objet</param>
        /// <returns>Objet dans le cache</returns>
        public T Get<T>(string key, int cacheTime, string patternKey, Func<T> acquire)
        {
            if (IsSet(key))
            {
                return Get<T>(key);
            }
            else
            {
                var result = acquire();
                Set(key, result, cacheTime, patternKey);
                return result;
            }
        }
    }
}
