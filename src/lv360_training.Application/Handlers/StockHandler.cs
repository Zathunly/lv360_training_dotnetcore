using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Repositories.Core;

namespace lv360_training.Application.Handlers;

public class StockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockHandler(IStockRepository stockRepository, IUnitOfWork unitOfWork)
    {
        _stockRepository = stockRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Stock?> GetByIdAsync(int id) =>
        await _stockRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Stock>> GetAllAsync() =>
        await _stockRepository.GetAllAsync();

    public async Task<Stock> CreateAsync(Stock stock)
    {
        stock.LastUpdatedAt = DateTime.UtcNow;

        await _stockRepository.AddAsync(stock);
        await _unitOfWork.SaveChangesAsync();

        return stock;
    }

    public async Task<Stock?> UpdateAsync(int id, Stock stock)
    {
        var existing = await _stockRepository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Quantity = stock.Quantity;
        existing.ProductId = stock.ProductId;
        existing.WarehouseId = stock.WarehouseId;
        existing.LastUpdatedAt = DateTime.UtcNow;

        _stockRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _stockRepository.GetByIdAsync(id);
        if (existing == null) return false;

        _stockRepository.Delete(existing);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Stock>> CreateBulkAsync(IEnumerable<Stock> stocks)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var s in stocks)
            s.LastUpdatedAt = utcNow;

        await _stockRepository.AddRangeAsync(stocks);
        await _unitOfWork.SaveChangesAsync();

        return stocks;
    }

    public async Task<IEnumerable<Stock>> UpdateBulkAsync(IEnumerable<Stock> stocks)
    {
        var allStocks = await _stockRepository.GetAllAsync();

        var existingStocks = allStocks
            .Where(s => stocks.Select(st => st.Id).Contains(s.Id))
            .ToDictionary(s => s.Id);

        var utcNow = DateTime.UtcNow;
        var toUpdate = new List<Stock>();

        foreach (var stock in stocks)
        {
            if (existingStocks.TryGetValue(stock.Id, out var existing))
            {
                // Update chỉ scalar properties
                existing.ProductId = stock.ProductId;
                existing.WarehouseId = stock.WarehouseId;
                existing.Quantity = stock.Quantity;
                existing.LastUpdatedAt = utcNow;

                toUpdate.Add(existing);
            }
        }

        // Update trong EF Core
        _stockRepository.UpdateRange(toUpdate);
        await _unitOfWork.SaveChangesAsync();

        return toUpdate;
    }

    public async Task<int> DeleteBulkAsync(IEnumerable<int> ids)
    {
        var stocks = (await _stockRepository.GetAllAsync())
            .Where(s => ids.Contains(s.Id))
            .ToList();

        if (!stocks.Any()) return 0;

        _stockRepository.DeleteRange(stocks);
        await _unitOfWork.SaveChangesAsync();

        return stocks.Count;
    }

}
