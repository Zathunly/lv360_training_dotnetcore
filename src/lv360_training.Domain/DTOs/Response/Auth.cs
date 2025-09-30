namespace lv360_training.Domain.Dtos.Auth.Response;

public class LoginResponse
{
    public string Message { get; set; } = default!;
    public string Username { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}

public class MeResponse
{
    public string Username { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
}
