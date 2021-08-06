using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;

        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            _db.StringSet("name", "Ali Bulut");
            _db.StringSet("visitor", 100);

            return View();
        }

        public async Task<IActionResult> ShowAsync()
        {
            var value = _db.StringGet("name");
            if (value.HasValue)
            {
                ViewBag.value = value.ToString();
            }

            _db.StringIncrement("visitor", 12);

            // both same
            var visitorCount = await _db.StringDecrementAsync("visitor", 2);
            var visitorCount2 = _db.StringDecrementAsync("visitor", 2).Result;

            // both same
            _db.StringDecrementAsync("visitor", 2).Wait();
            await _db.StringDecrementAsync("visitor", 2);

            var value2 = _db.StringGetRange("name", 0, 3);
            var value3 = _db.StringLength("name");

            // to store file as bytes (we can type any types of value to StringSet in order to store in redis)
            Byte[] imageByte = default(byte[]);
            _db.StringSet("image", imageByte);

            return View();
        }
    }
}
