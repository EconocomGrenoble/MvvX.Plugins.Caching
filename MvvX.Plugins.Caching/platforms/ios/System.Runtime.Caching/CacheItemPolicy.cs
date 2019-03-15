using System;
using System.Collections.Generic;

namespace MvvX.Plugins.Caching.System.Runtime.Caching
{
    internal class CacheItemPolicy
    {
        public DateTimeOffset AbsoluteExpiration { get; set; }

        public IList<CacheEntryChangeMonitor> ChangeMonitors { get; internal set; }

        public CacheItemPolicy()
        {
            ChangeMonitors = new List<CacheEntryChangeMonitor>();
        }
    }
}