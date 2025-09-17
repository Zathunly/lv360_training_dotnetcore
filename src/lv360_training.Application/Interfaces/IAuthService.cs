namespace lv360_training.Application.Interfaces;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
