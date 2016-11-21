using System;
using MvvmCross.Platform;
using MvvmCross.Platform.Plugins;

namespace MvvX.Plugins.Caching.Wpf
{
    public class Plugin : IMvxConfigurablePlugin
    {
        private string configurationKey;

        public void Configure(IMvxPluginConfiguration configuration)
        {
            var config = configuration as PluginConfiguration;
            if (config == null || string.IsNullOrWhiteSpace(config.GlobalCacheKey))
                throw new ArgumentNullException("configuration is invalid. null or globalKey is null or white spaces");

            configurationKey = config.GlobalCacheKey;
        }

        public void Load()
        {
            Mvx.RegisterSingleton<ICacheManager>(new CacheManager(configurationKey));
        }
    }
}
