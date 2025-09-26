namespace lv360_training.Domain.Dtos.Auth.Request;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Message { get; set; } = default!;
    public string Username { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
