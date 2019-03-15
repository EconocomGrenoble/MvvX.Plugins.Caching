using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvvX.Plugins.Caching.Wpf.UnitTests
{
    /// <summary>
    /// Description résumée pour CacheManagerTest
    /// </summary>
    [TestClass]
    public class CacheManagerTest
    {
        private int durationCache = 60;

        [TestMethod]
        public void CacheManager_GetSetFromCache()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            
            cacheManager.Clear();
            
            for (int i = 1; i < 10; i++)
                cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");
            
            Thread.Sleep(2000);
            
            Assert.IsTrue(cacheManager.IsSet("keys"));
            
            for (int i = 1; i < 10; i++)
            {
                Assert.IsTrue(cacheManager.IsSet("key_" + i.ToString()));
                Assert.IsTrue(cacheManager.Get<int>("key_" + i.ToString()) == i * i);
            }
        }

        [TestMethod]
        public void CacheManager_GetWithSet()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            
            cacheManager.Clear();
            
            for (int i = 1; i < 10; i++)
                cacheManager.Get("GetWithSetkey_" + i.ToString(), "GetWithSetkeys", () =>
                {
                    return i * i;
                });
            
            Assert.IsTrue(cacheManager.IsSet("GetWithSetkeys"));
            
            for (int i = 1; i < 10; i++)
            {
                Assert.IsTrue(cacheManager.IsSet("GetWithSetkey_" + i.ToString()));
                Assert.IsTrue(cacheManager.Get<int>("GetWithSetkey_" + i.ToString()) == i * i);
            }
        }

        [TestMethod]
        public void CacheManager_GetWithSetCacheTime()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            
            cacheManager.Clear();
            
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

        [TestMethod]
        public void CacheManager_ClearCache()
        {
            var cacheManager = new CacheManager("Test.Caching.Wpf");
            
            cacheManager.Clear();
            
            for (int i = 1; i < 10; i++)
                cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");
            
            cacheManager.Clear();
            
            Assert.IsFalse(cacheManager.IsSet("keys"));
            for (int i = 1; i < 10; i++)
                Assert.IsFalse(cacheManager.IsSet("key_" + i.ToString()));
        }

        // TODO : Fix this unit test
        //[TestMethod]
        //public void CacheManager_TestPatternKey()
        //{
        //    var cacheManager = new CacheManager("Test.Caching.Wpf");

        //    cacheManager.Clear();
            
        //    for (int i = 1; i < 10; i++)
        //        cacheManager.Set("key_" + i.ToString(), i * i, durationCache, "keys");

        //    cacheManager.Remove("keys");

        //    Assert.IsFalse(cacheManager.IsSet("keys"), "keys is set, not removed");
        //    for (int i = 1; i < 10; i++)
        //        Assert.IsFalse(cacheManager.IsSet("key_" + i.ToString()), "key_" + i.ToString() + " is set, not removed");
        //}
    }
}
