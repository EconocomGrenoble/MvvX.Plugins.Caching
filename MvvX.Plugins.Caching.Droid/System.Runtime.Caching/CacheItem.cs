using System.Collections.Generic;

namespace MvvX.Plugins.Caching.Droid.System.Runtime.Caching
{
    internal class CacheItem
    {
        public string CacheKey { get; internal set; }

        public object Value { get; internal set; }

        public CacheItem(string cacheKey, object value)
        {
            CacheKey = cacheKey;
            Value = value;
        }
    }
}