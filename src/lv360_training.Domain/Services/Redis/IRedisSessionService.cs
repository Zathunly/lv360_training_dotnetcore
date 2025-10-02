namespace lv360_training.Domain.Services.Redis
{
    public interface IRedisSessionService
    {
        TimeSpan Ttl { get; }
        TimeSpan Threshold { get; }
        Task CreateSessionAsync(string sessionId, RedisSession session, TimeSpan ttl);

        Task RenewSessionAsync(string sessionId);

        Task<RedisSession?> GetSessionAsync(string sessionId);

        Task DeleteSessionAsync(string sessionId);

        Task DeleteSessionByUserIdAsync(int userId);
    }
}
