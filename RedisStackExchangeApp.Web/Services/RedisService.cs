using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RedisStackExchangeApp.Web.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _redisPort;
        private ConnectionMultiplexer _redis;
        public IDatabase db { get; set; }

        public RedisService(IConfiguration configuration)
        {
            // to get fields in appsettings.json
            _redisHost = configuration["Redis:Host"];
            _redisPort = configuration["Redis:Port"];
        }

        public void Connect()
        {
            var configString = $"{_redisHost}:{_redisPort}";
            _redis = ConnectionMultiplexer.Connect(configString);
        }

        // There are 16 dbs by default in redis from 0 to 15. We can choose which one we want to work on.
        public IDatabase GetDb(int dbNum)
        {
            return _redis.GetDatabase(dbNum);
        }
    }
}
