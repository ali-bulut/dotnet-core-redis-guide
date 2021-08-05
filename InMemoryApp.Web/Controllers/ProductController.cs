using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        // we can reach InMemoryCache related methods by using IMemoryCache interface.
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            // first way to check if time is in memory
            //if (string.IsNullOrEmpty(_memoryCache.Get<string>("time"))){
            //    _memoryCache.Set<string>("time", DateTime.Now.ToString());
            //}

            // second way
            // if there is already time in memory, timeCache is the value of time.
            if (!(_memoryCache.TryGetValue<string>("time", out string timeCache)))
            {
                // setting cache properties
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
                options.SlidingExpiration = TimeSpan.FromSeconds(10);

                // CacheItemPriority.Low, CacheItemPriority.Normal, CacheItemPriority.High, CacheItemPriority.NeverRemove
                // when the memory is full, the program will start to remove keys by checking at priorities of the keys.
                // If we set all keys' priorities as NeverRemove and program tries to add new data to memory when memory is full, it will raise an exception
                // because program cannot remove any of the old keys and so cannot add new data.
                options.Priority = CacheItemPriority.High;

                // this method is fired when the key is removed from the memory.
                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    // reason stores that why this key is removed from the memory. (it can be removed because of the expiration time or when the memory is full)
                    _memoryCache.Set("callback", $"{key} -> {value} => Reason: {reason}");
                });

                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);

                Product p = new Product() { Id = 1, Name = "Pencil", Price = 20 };
                // we don't need to do serialize/deserialize operations. IMemoryCache does it automatically.
                _memoryCache.Set<Product>("product:1", p);
            }

            // third way => if there is already time in memory, it will return the time, if there is not, it will set
            // time by using the second parameter and return it.
            //_memoryCache.GetOrCreate<string>("time", entry =>
            //{
            //    // by using entry we can set the cache properties such as lifespan or etc.
            //    return DateTime.Now.ToString();
            //});

            // it will remove time from the memory.
            //_memoryCache.Remove("time");

            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue<string>("time", out string timeCache);
            _memoryCache.TryGetValue<string>("callback", out string callback);
            //ViewBag.time = _memoryCache.Get<string>("time");
            ViewBag.time = timeCache;
            ViewBag.callback = callback;
            ViewBag.product = _memoryCache.Get<Product>("product:1");

            return View();
        }
    }
}
