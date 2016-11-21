using MvvmCross.Platform.Plugins;

namespace MvvX.Plugins.Caching
{
    public class PluginConfiguration : IMvxPluginConfiguration
    {
        /// <summary>
        /// Define the key use to be the global cache key
        /// </summary>
        public string GlobalCacheKey { get; set; }
    }
}
