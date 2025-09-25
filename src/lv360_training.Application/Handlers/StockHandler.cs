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
}
