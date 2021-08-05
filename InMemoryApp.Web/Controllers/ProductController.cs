using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (string.IsNullOrEmpty(_memoryCache.Get<string>("time"))){
                _memoryCache.Set<string>("time", DateTime.Now.ToString());
            }

            // second way
            if(!(_memoryCache.TryGetValue<string>("time", out string timeCache)))
            {
                // if there is already time in memory, timeCache is the value of time.
                _memoryCache.Set<string>("time", DateTime.Now.ToString());
            }

            // third way => if there is already time in memory, it will return the time, if there is not, it will set
            // time by using the second parameter and return it.
            _memoryCache.GetOrCreate<string>("time", entry =>
            {
                // by using entry we can set the cache properties such as lifespan or etc.
                return DateTime.Now.ToString();
            });

            // it will remove time from the memory.
            //_memoryCache.Remove("time");

            return View();
        }

        public IActionResult Show()
        {
            ViewBag.time = _memoryCache.Get<string>("time");
            return View();
        }
    }
}
