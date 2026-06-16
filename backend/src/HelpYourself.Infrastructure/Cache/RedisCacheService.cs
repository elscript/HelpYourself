using System.Text.Json;
using HelpYourself.Core.Enums;
using HelpYourself.Core.Interfaces;
using HelpYourself.Core.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace HelpYourself.Infrastructure.Cache;

public sealed class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<Ritual?> GetCachedRitualAsync(Archetype archetype, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = CacheKey(archetype);
            var value = await db.StringGetAsync(key);
            if (!value.HasValue) return null;
            return JsonSerializer.Deserialize<Ritual>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis read failed for archetype {Archetype}", archetype);
            return null;
        }
    }

    public async Task SetCachedRitualAsync(Archetype archetype, Ritual ritual, CancellationToken ct = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = CacheKey(archetype);
            var json = JsonSerializer.Serialize(ritual);
            await db.StringSetAsync(key, json, CacheTtl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis write failed for archetype {Archetype}", archetype);
        }
    }

    private static string CacheKey(Archetype archetype) =>
        $"ritual:instant:{archetype.ToString().ToLowerInvariant()}";
}
