using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class HashTypeController : BaseController
    {
        private string hashKey = "hashnames";

        public HashTypeController(RedisService redisService) : base(redisService, 4)
        {
        }

        public IActionResult Index()
        {
            Dictionary<string, string> hashList = new Dictionary<string, string>();

            if (_db.KeyExists(hashKey))
            {
                _db.HashGetAll(hashKey).ToList().ForEach(x =>
                {
                    // name ~= key
                    hashList.Add(x.Name, x.Value);
                });
            }

            return View(hashList);
        }

        [HttpPost]
        public IActionResult Add(string name, string value)
        {
            _db.HashSet(hashKey, name, value);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string name)
        {
            await _db.HashDeleteAsync(hashKey, name);
            return RedirectToAction("Index");
        }
    }
}
