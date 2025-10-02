namespace lv360_training.Domain.Services.Redis
    {
    public class RedisSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
