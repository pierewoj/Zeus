using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Zeus.Crawler
{
    public interface IRedisProvider
    {
        IDatabase GetDatabase();
    }

    class RedisProvider : IRedisProvider
    {
        private readonly ILogger<RedisProvider> _logger;

        public RedisProvider(ILogger<RedisProvider> logger)
        {
            _logger = logger;
        }

        public IDatabase GetDatabase()
        {
            IPHostEntry ips = Dns.GetHostEntryAsync(Configuration.RedisHost).Result;
            var ip = ips.AddressList.First().ToString();
            _logger.LogInformation($"Redis server address was resolved to {ip}");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(ip);
            return redis.GetDatabase();
        }
    }
}
