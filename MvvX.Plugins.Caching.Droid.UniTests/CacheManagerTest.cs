using System;
using NUnit.Framework;
using System.Threading;

namespace MvvX.Plugins.Caching.Droid.UniTests
{
    /// <summary>
    /// Description résumée pour CacheManagerTest
    /// </summary>
    [TestFixture]
    public class CacheManagerTest
    {
        private int durationCache = 60;

        [Test]
        public void CacheManager_GetSetFromCache()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            // On vide le cache
            cacheManager.Clear();

            // On rajoute les éléments dans le cache :
            for (int i = 1; i < 10; i++)
                cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");

            Thread.Sleep(5000);

            // On va vérifier maintenant que les éléments y sont bien, et que la masterkey existe bien :
            Assert.IsTrue(cacheManager.IsSet("keys"));

            for (int i = 1; i < 10; i++)
            {
                Assert.IsTrue(cacheManager.IsSet("key_" + i.ToString()));
                Assert.IsTrue(cacheManager.Get<int>("key_" + i.ToString()) == i * i);
            }
        }

        [Test]
        public void CacheManager_GetWithSet()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            // On vide le cache
            cacheManager.Clear();

            // On rajoute les éléments dans le cache :
            for (int i = 1; i < 10; i++)
                cacheManager.Get("GetWithSetkey_" + i.ToString(), "GetWithSetkeys", () =>
                {
                    return i * i;
                });

            // On va vérifier maintenant que les éléments y sont bien, et que la masterkey existe bien :
            Assert.IsTrue(cacheManager.IsSet("GetWithSetkeys"));
            for (int i = 1; i < 10; i++)
            {
                Assert.IsTrue(cacheManager.IsSet("GetWithSetkey_" + i.ToString()));
                Assert.IsTrue(cacheManager.Get<int>("GetWithSetkey_" + i.ToString()) == i * i);
            }
        }

        [Test]
        public void CacheManager_GetWithSetCacheTime()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            // On vide le cache
            cacheManager.Clear();

            // On rajoute les éléments dans le cache :
            for (int i = 1; i < 10; i++)
            {
                int cacheValue = cacheManager.Get("GetWithSetCacheTimekey_" + i.ToString(), 25, "GetWithSetCacheTimekeys", () =>
                {
                    return i * i;
                });
                Assert.IsTrue((cacheValue == i * i));
                int cacheValueNext = cacheManager.Get("GetWithSetCacheTimekey_" + i.ToString(), 25, "GetWithSetCacheTimekeys", () =>
                {
                    return i * i;
                });
                Assert.IsTrue((cacheValue == cacheValueNext));
            }
        }

        [Test]
        public void CacheManager_ClearCache()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            // On vide le cache
            cacheManager.Clear();

            // On rajoute les éléments dans le cache :
            for (int i = 1; i < 10; i++)
                cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");

            // On vide le cache
            cacheManager.Clear();

            // On vérifie que les clés n'y sont plus
            Assert.IsFalse(cacheManager.IsSet("keys"));
            for (int i = 1; i < 10; i++)
                Assert.IsFalse(cacheManager.IsSet("key_" + i.ToString()));
        }

        [Test]
        public void CacheManager_TestPatternKey()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            // On vide le cache
            cacheManager.Clear();

            // On rajoute les éléments dans le cache :
            for (int i = 1; i < 10; i++)
                cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");

            cacheManager.Remove("keys");

            // On vérifie que les clés n'y sont plus
            Assert.IsFalse(cacheManager.IsSet("keys"));
            for (int i = 1; i < 10; i++)
                Assert.IsFalse(cacheManager.IsSet("key_" + i.ToString()));
        }
    }
}
