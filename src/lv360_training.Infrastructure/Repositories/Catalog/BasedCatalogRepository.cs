using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories.Catalog;

public class BasedCatalogRepository<T> : IBasedCatalogRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public BasedCatalogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await _context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity) =>
        await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) =>
        _context.Set<T>().Update(entity);

    public void Delete(T entity) =>
        _context.Set<T>().Remove(entity);
}
