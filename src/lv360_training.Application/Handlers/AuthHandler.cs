using lv360_training.Application.Interfaces;
using lv360_training.Domain;

namespace lv360_training.Application.Handlers;

public class AuthHandler
{
    private readonly IDbService _db;
    private readonly IAuthService _auth;

    public AuthHandler(IDbService db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    // Register method
    public async Task<int> Register(string username, string password)
    {
        if (_db.Users.Any(u => u.Username == username))
            throw new Exception("User already exists");

        var user = new User
        {
            Username = username,
            Password = _auth.HashPassword(password)
        };

        await _db.AddUserAsync(user);
        await _db.SaveChangesAsync();
        return user.Id;
    }

    // Login method
    public string Login(string username, string password)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null || !_auth.VerifyPassword(password, user.Password))
            throw new Exception("Invalid username or password");

        return _auth.GenerateJwtToken(user.Username);
    }
}
