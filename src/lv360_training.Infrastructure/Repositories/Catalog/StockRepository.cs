using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories.Catalog;

public class StockRepository : IStockRepository
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

    public async Task<Stock?> GetByProductIdAsync(int productId)
    {
        return await _context.Stocks
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.ProductId == productId);
    }

    public async Task<Stock?> GetByProductAndWarehouseAsync(int productId, int warehouseId)
    {
        return await _context.Stocks
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
    }

    public async Task AddAsync(Stock entity) =>
        await _context.Stocks.AddAsync(entity);

    public void Update(Stock entity) =>
        _context.Stocks.Update(entity);

    public void Delete(Stock entity) =>
        _context.Stocks.Remove(entity);

    public async Task AddRangeAsync(IEnumerable<Stock> entities) =>
        await _context.Stocks.AddRangeAsync(entities);

    public void UpdateRange(IEnumerable<Stock> entities) =>
        _context.Stocks.UpdateRange(entities);

    public void DeleteRange(IEnumerable<Stock> entities) =>
        _context.Stocks.RemoveRange(entities);

}
