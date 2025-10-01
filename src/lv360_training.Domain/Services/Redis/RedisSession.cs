namespace lv360_training.Domain.Services.Redis
{
    public class RedisSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
}
