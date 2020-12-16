using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.OutputCache.Core.Cache;

namespace DotneterWhj.Caching
{
    public class RedisCacheProvider : IApiOutputCache
    {
        private readonly ConnectionMultiplexer _redis;

        private readonly IDatabase _db;

        public RedisCacheProvider(ConnectionMultiplexer redis)
        {
            this._redis = redis;
            _db = _redis.GetDatabase();
        }

        public IEnumerable<string> AllKeys
        {
            get
            {
                return _redis.GetServer(_redis.Configuration).Keys(_db.Database).Select(r => r.ToString());
            }
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            var timeSpan = expiration.Subtract(new DateTimeOffset(DateTime.Now));

            var value = Newtonsoft.Json.JsonConvert.SerializeObject(o);

            _db.StringSet(key, value, timeSpan);
        }

        public bool Contains(string key)
        {
            return _db.KeyExists(key);
        }

        public T Get<T>(string key) where T : class
        {
            var value = _db.StringGet(key);

            if (string.IsNullOrEmpty(value))
                return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        public object Get(string key)
        {
            return _db.StringGet(key);
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }

        public void RemoveStartsWith(string key)
        {
            var keys = _redis.GetServer(_redis.Configuration).Keys(_db.Database, key);

            foreach (var item in keys)
            {
                _db.KeyDelete(item);
            }
        }
    }
}