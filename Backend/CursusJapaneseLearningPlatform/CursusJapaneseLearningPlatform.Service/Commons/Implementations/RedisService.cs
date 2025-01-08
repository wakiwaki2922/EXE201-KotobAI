using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations
{
    

    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisService(IConfiguration configuration)
        {
            var options = ConfigurationOptions.Parse($"{configuration["Redis:Host"]}:{configuration["Redis:Port"]}");
            options.Password = configuration["Redis:Password"];
            _redis = ConnectionMultiplexer.Connect(options);
        }

        public async Task SetValueAsync(string key, string value)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value);
        }

        public async Task<string> GetValueAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }
    }
}
