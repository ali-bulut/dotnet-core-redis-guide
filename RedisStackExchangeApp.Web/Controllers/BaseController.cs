using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisStackExchangeApp.Web.Services;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly RedisService _redisService;
        protected readonly IDatabase _db;

        public BaseController(RedisService redisService, int dbNum)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(dbNum);
        }
    }
}
