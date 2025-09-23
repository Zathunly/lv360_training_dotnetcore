using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories.Catalog;

public class StockRepository : IBasedCatalogRepository<Stock>
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Stock?> GetByIdAsync(int id) =>
        await _context.Stocks
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Stock>> GetAllAsync() =>
        await _context.Stocks
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .ToListAsync();

    public async Task AddAsync(Stock entity) =>
        await _context.Stocks.AddAsync(entity);

    public void Update(Stock entity) =>
        _context.Stocks.Update(entity);

    public void Delete(Stock entity) =>
        _context.Stocks.Remove(entity);
}
