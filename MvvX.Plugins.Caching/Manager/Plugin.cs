using MvvmCross;
using MvvmCross.Plugin;
using System;

namespace MvvX.Plugins.Caching
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxConfigurablePlugin
    {
        private string configurationKey;

        public void Configure(IMvxPluginConfiguration configuration)
        {
            var config = configuration as PluginConfiguration;
            if (config == null || string.IsNullOrWhiteSpace(config.GlobalCacheKey))
                throw new ArgumentNullException(nameof(configuration), "configuration is invalid. null or globalKey is null or white spaces");

            configurationKey = config.GlobalCacheKey;
        }

        public void Load()
        {
            Mvx.IoCProvider.RegisterSingleton<ICacheManager>(new CacheManager(configurationKey));
        }
    }
}
