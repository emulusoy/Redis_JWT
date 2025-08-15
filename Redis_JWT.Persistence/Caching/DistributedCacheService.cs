using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Redis_JWT.Application.Abstractions;

namespace Redis_JWT.Persistence.Caching
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var data = await _cache.GetAsync(key, ct);
            return data is null ? default : JsonSerializer.Deserialize<T>(data, _json);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            _cache.RemoveAsync(key, ct);        
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _json);
            var opt = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(5)
            };
            await _cache.SetAsync(key, bytes, opt, ct);
        }
    }
}
