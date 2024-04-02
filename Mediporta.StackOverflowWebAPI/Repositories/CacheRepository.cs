using Mediporta.StackOverflowWebAPI.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Mediporta.StackOverflowWebAPI.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _cache;

        public CacheRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T GetData<T>(string key)
        {
            var value = _cache.GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public void SetData<T>(string key, T value)
        {
            _cache.SetString(key, JsonConvert.SerializeObject(value));
        }
        public void RemoveData(string key)
        {
            _cache.Remove(key);
        }
    }
}
