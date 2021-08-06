﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private string listKey = "names";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(1);
        }

        public IActionResult Index()
        {
            List<string> namesList = new List<string>();
            if (_db.KeyExists(listKey))
            {
                _db.ListRange(listKey).ToList().ForEach(x => {
                    namesList.Add(x.ToString());
                });
            }

            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            _db.ListRightPush(listKey, name);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(string name)
        {
            _db.ListRemoveAsync(listKey, name).Wait();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFirst()
        {
            _db.ListLeftPop(listKey);
            return RedirectToAction("Index");
        }
    }
}
