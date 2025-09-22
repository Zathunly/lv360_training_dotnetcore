namespace lv360_training.Domain.Interfaces.Security;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
