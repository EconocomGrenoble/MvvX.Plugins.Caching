using System;

namespace MvvX.Plugins.Caching
{
    public interface ICacheManager
    {
        /// <summary>
        /// Get an item in the cache from his key
        /// </summary>
        /// <typeparam name="T">Type of returned object</typeparam>
        /// <param name="cacheKey">key to find</param>
        /// <returns>Object finded, default(T) if not finded</returns>
        T Get<T>(string cacheKey);
        
        /// <summary>
        /// Indique si une élément existe dans le cache pour la clée donnée
        /// </summary>
        /// <param name="cacheKey">key to find</param>
        /// <returns></returns>
        bool IsSet(string cacheKey);

        /// <summary>
        /// Try to get an object in cache from his key
        /// </summary>
        /// <param name="cacheKey">key to find</param>
        /// <returns></returns>
        object RetrieveThis(string cacheKey);

        /// <summary>
        /// Check if an object is null in the cache by his key
        /// </summary>
        /// <param name="cacheKey">Key to be tested</param>
        /// <returns>True is the object if null</returns>
        bool IsThisNull(string cacheKey);

        /// <summary>
        /// Delete an entry by his key
        /// </summary>
        /// <param name="cacheKey">key to find</param>
        void Remove(string cacheKey);

        /// <summary>
        /// Clear all data in cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Get an item in the cache.
        /// Execute a method to insert the object if the content cache is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">key to find</param>
        /// <param name="patternKey">key pattern, used to clear items in cache by pattern</param>
        /// <param name="acquire">Function used to insert the item in cache if it not exists</param>
        /// <returns></returns>
        T Get<T>(string cacheKey, string patternKey, Func<T> acquire);

        /// <summary>
        /// Get an item in the cache.
        /// Execute a method to insert the object if the content cache is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">key to find</param>
        /// <param name="cacheTime">Time in second for the cache item availability before being deleted</param>
        /// <param name="patternKey">key pattern, used to clear items in cache by pattern</param>
        /// <param name="acquire">Function used to insert the item in cache if it not exists</param>
        /// <returns></returns>
        T Get<T>(string key, int cacheTime, string patternKey, Func<T> acquire);
    }
}
