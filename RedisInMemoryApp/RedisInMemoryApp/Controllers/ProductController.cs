using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RedisInMemoryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisInMemoryApp.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            #region [SET KEY INMEMORY]
            //SetKeyInMemory();
            #endregion


            #region [ABSOLUTE EXPIRATION]
            //AbsoluteExpiration();
            #endregion


            #region [SLIDING EXPIRATION]
            // SlidingExpiration();
            #endregion


            #region [CACHE PRIORITY]
            // CachePriority();
            #endregion


            #region [REGISTERPOSTEVICTIONCALLBACK]
            //RegisterPostEvictionCallback();
            #endregion


            #region [COMPLEX TYPES CACHING]
           // ComplexTypesCache();
            #endregion
            return View();
        }


        public IActionResult Show()
        {
            #region [REMOVE MEMORY]
            //RemoveKey();
            #endregion

            #region [GET VALUE INMEMORY]
            //string zaman = GetValueInMemory();
            //ViewBag.zaman = zaman;
            #endregion


            _memoryCache.TryGetValue("zaman", out string zamancache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.zaman = zamancache;
            ViewBag.callback = callback;
            ViewBag.product = _memoryCache.Get<Product>("product:1");
            return View();
        }


        private void SetKeyInMemory()
        {
            //1.yol
            if (string.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            }

            //2.yol
            if (!_memoryCache.TryGetValue("zaman", out string zamancache))
            {

                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            }
        }

        private string GetValueInMemory()
        {
            _memoryCache.GetOrCreate<string>("zaman", entry => { return DateTime.Now.ToString(); });  // varsa alır yoksa oluşturur.
            return _memoryCache.Get<string>("zaman");
        }

        private void RemoveKey()
        {

            _memoryCache.Remove("zaman");   //key silinir
        }


        private void AbsoluteExpiration()
        {
            if (!_memoryCache.TryGetValue("zaman", out string zamancache))
            {

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
            }
        }

        private void SlidingExpiration()
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.SlidingExpiration = TimeSpan.FromSeconds(10);
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
        }

        private void CachePriority()
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.Priority = CacheItemPriority.High;
            //High : Ram dolarsa en son bunu sil 
            //Low : Ram dolarsa ilk bunu sil 
            //NeverRemove : Ram dolsa da silme Not: Exception'a düşme ihtimali var Ram dolarsa
            //Normal : High ile Low arasında
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
        }

        private void RegisterPostEvictionCallback()
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.Priority = CacheItemPriority.High;
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key}-->{value}=> sebep :{reason}");
            });
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);
        }

        private void ComplexTypesCache()
        {
            Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };
            _memoryCache.Set<Product>("product:1", p);  //Serialize otomatik yapıyor.
        }
    }
}
