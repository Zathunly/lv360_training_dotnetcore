using lv360_training.Application.Interfaces;
using lv360_training.Domain;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Db;

public class DbService : IDbService
{
    private readonly AppDbContext _context;

    public DbService(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<User> Users => _context.Users;

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
