using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;

        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            _distributedCache.SetString("name", "Ali", cacheEntryOptions);
            await _distributedCache.SetStringAsync("surname", "Bulut", cacheEntryOptions);

            Product p = new Product() { Id = 1, Name = "pen", Price = 10 };

            // serialize method (recommended)
            string jsonProduct = JsonSerializer.Serialize(p);
            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheEntryOptions);

            // binary method
            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);
            _distributedCache.Set("product:1:byte", byteProduct, cacheEntryOptions);

            return View();
        }

        public async Task<IActionResult> ShowAsync()
        {
            string name = _distributedCache.GetString("name");
            ViewBag.name = name;

            string surname = await _distributedCache.GetStringAsync("surname");
            ViewBag.surname = surname;

            // deserialize method (recommended)
            string jsonProduct = _distributedCache.GetString("product:1");
            if (!string.IsNullOrEmpty(jsonProduct))
            {
                Product p = JsonSerializer.Deserialize<Product>(jsonProduct);
                ViewBag.product = p;
            }
            
            // binary method
            Byte[] byteProduct = _distributedCache.Get("product:1:byte");
            if (byteProduct != null)
            {
                string stringProduct = Encoding.UTF8.GetString(byteProduct);
                Product p2 = JsonSerializer.Deserialize<Product>(stringProduct);
                ViewBag.product2 = p2;
            }

            return View();
        }

        public async Task<IActionResult> RemoveAsync()
        {
            _distributedCache.Remove("name");
            await _distributedCache.RemoveAsync("surname");
            _distributedCache.Remove("product:1");
            _distributedCache.Remove("product:1:byte");

            return View();
        }
    }
}
