using System.Text.Json;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Infrastructure.Caching;

public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedString = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(cachedString))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedString, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}

