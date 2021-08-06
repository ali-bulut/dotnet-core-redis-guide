using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class SetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private string setKey = "hashnames";

        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(2);
        }

        public IActionResult Index()
        {
            HashSet<string> namesList = new HashSet<string>();

            if (_db.KeyExists(setKey))
            {
                _db.SetMembers(setKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });
            }

            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            // There is no such a feature called sliding time in redis. Instead of it, we can use KeyExpire.
            // So in every request to that method, this key's expire time will be reseted and set to 5 minutes.
            _db.KeyExpire(setKey, DateTime.Now.AddMinutes(5));

            // if we just want to set only 5 minutes and don't want to reset in every request, we can use that.
            if (!_db.KeyExists(setKey))
            {
                _db.KeyExpire(setKey, DateTime.Now.AddMinutes(5));
            }
            _db.SetAdd(setKey, name);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string name)
        {
            await _db.SetRemoveAsync(setKey, name);
            return RedirectToAction("Index");
        }
    }
}
