using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using lv360_training.Domain.Services.Redis;

namespace lv360_training.Infrastructure.Services.Redis;

public class RedisSessionService : IRedisSessionService
{
    private readonly IDistributedCache _cache;

    public RedisSessionService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private string GetKey(string sessionId) => $"session:{sessionId}";
    private string GetUserKey(int userId) => $"user_session:{userId}";

    public async Task CreateSessionAsync(string sessionId, RedisSession session, TimeSpan ttl)
    {
        var oldSessionId = await _cache.GetStringAsync(GetUserKey(session.UserId));
        if (!string.IsNullOrEmpty(oldSessionId))
            await _cache.RemoveAsync(GetKey(oldSessionId));

        var data = JsonSerializer.Serialize(session);
        await _cache.SetStringAsync(GetKey(sessionId), data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });

        await _cache.SetStringAsync(GetUserKey(session.UserId), sessionId, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });
    }

    public async Task<RedisSession?> GetSessionAsync(string sessionId)
    {
        var data = await _cache.GetStringAsync(GetKey(sessionId));
        return data == null ? null : JsonSerializer.Deserialize<RedisSession>(data);
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        await _cache.RemoveAsync(GetKey(sessionId));
    }

    public async Task DeleteSessionByUserIdAsync(int userId)
    {
        var sessionId = await _cache.GetStringAsync(GetUserKey(userId));
        if (!string.IsNullOrEmpty(sessionId))
        {
            await _cache.RemoveAsync(GetKey(sessionId));
            await _cache.RemoveAsync(GetUserKey(userId));
        }
    }
}
