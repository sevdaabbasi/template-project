using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace BuildingBlocks.Infrastructure.Authentication;

public sealed class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _cache;

    public TokenBlacklistService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task BlacklistTokenAsync(string token, TimeSpan timeToLive)
    {
        _cache.Set(token, true, timeToLive);
        return Task.CompletedTask;
    }

    public Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return Task.FromResult(_cache.TryGetValue(token, out _));
    }
}
