using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using lv360_training.Domain.Services.Redis;

namespace lv360_training.Infrastructure.Services.Redis
{
    public class RedisSessionService : IRedisSessionService
    {
        private readonly IDistributedCache _cache;
        public TimeSpan Ttl { get; }
        public TimeSpan Threshold { get; }

        public RedisSessionService(IDistributedCache cache)
        {
            _cache = cache;

            var ttlSeconds = Environment.GetEnvironmentVariable("REDIS_TTL_SECONDS") ?? "1800";
            var thresholdSeconds = Environment.GetEnvironmentVariable("REDIS_THRESHOLD_SECONDS") ?? "300";

            Ttl = TimeSpan.FromSeconds(int.Parse(ttlSeconds));
            Threshold = TimeSpan.FromSeconds(int.Parse(thresholdSeconds));
        }
        
        private string GetKey(string sessionId) => $"session:{sessionId}";
        private string GetUserKey(int userId) => $"user_session:{userId}";

        private async Task SaveSessionAsync(string sessionId, RedisSession session, TimeSpan ttl)
        {
            var data = JsonSerializer.Serialize(session);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await _cache.SetStringAsync(GetKey(sessionId), data, options);
            await _cache.SetStringAsync(GetUserKey(session.UserId), sessionId, options);
        }

        public async Task CreateSessionAsync(string sessionId, RedisSession session, TimeSpan ttl)
        {
            var oldSessionId = await _cache.GetStringAsync(GetUserKey(session.UserId));
            if (!string.IsNullOrEmpty(oldSessionId))
                await _cache.RemoveAsync(GetKey(oldSessionId));

            session.CreatedAt = DateTime.UtcNow;
            session.ExpiresAt = DateTime.UtcNow.Add(ttl);

            await SaveSessionAsync(sessionId, session, ttl);
        }

        public async Task RenewSessionAsync(string sessionId)
        {
            var session = await GetSessionAsync(sessionId);
            if (session == null) return;

            var remaining = session.ExpiresAt - DateTime.UtcNow;

            if (remaining <= Threshold)
            {
                session.ExpiresAt = DateTime.UtcNow.Add(Ttl);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = Ttl
                };

                await _cache.SetStringAsync(GetKey(sessionId), JsonSerializer.Serialize(session), options);
                await _cache.SetStringAsync(GetUserKey(session.UserId), sessionId, options);
            }
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
}
