namespace lv360_training.Domain.Services.Redis
{

    public interface IRedisSessionService
    {
        Task CreateSessionAsync(string sessionId, RedisSession session, TimeSpan ttl);
        Task<RedisSession?> GetSessionAsync(string sessionId);
        Task DeleteSessionAsync(string sessionId);
        Task DeleteSessionByUserIdAsync(int userId);
    }
}
