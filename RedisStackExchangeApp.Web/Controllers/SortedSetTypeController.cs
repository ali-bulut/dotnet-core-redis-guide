using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class SortedSetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private string sortedSetKey = "sortedsetnames";

        public SortedSetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(3);
        }

        public IActionResult Index()
        {
            HashSet<string> namesList = new HashSet<string>();

            if (_db.KeyExists(sortedSetKey))
            {
                _db.SortedSetScan(sortedSetKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });

                // we can also sort the set by value's score.
                //_db.SortedSetRangeByRank(sortedSetKey, order: Order.Ascending).ToList().ForEach(x =>
                //{
                //    namesList.Add(x.ToString());
                //});
            }

            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            _db.SortedSetAdd(sortedSetKey, name, score);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(string name)
        {
            // name = some_name:1 (value:score)
            _db.SortedSetRemove(sortedSetKey, name.Split(":")[0]);
            return RedirectToAction("Index");
        }
    }
}
