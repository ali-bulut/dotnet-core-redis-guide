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
            _memoryCache.Set<string>("time", DateTime.Now.ToString());
            return View();
        }

        public IActionResult Show()
        {
            ViewBag.time = _memoryCache.Get<string>("time");
            return View();
        }
    }
}
