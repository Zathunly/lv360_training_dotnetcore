using lv360_training.Infrastructure.Db;
using lv360_training.Application.Interfaces.Repositories.Core;

namespace lv360_training.Infrastructure.Repositories.Core;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
