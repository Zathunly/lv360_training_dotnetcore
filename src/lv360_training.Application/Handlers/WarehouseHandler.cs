using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Repositories.Core;

namespace lv360_training.Application.Handlers;

public class WarehouseHandler
{
    private readonly IBasedCatalogRepository<Warehouse> _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseHandler(IBasedCatalogRepository<Warehouse> warehouseRepository, IUnitOfWork unitOfWork)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Warehouse?> GetByIdAsync(int id) =>
        await _warehouseRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Warehouse>> GetAllAsync() =>
        await _warehouseRepository.GetAllAsync();

    public async Task<Warehouse> CreateAsync(Warehouse warehouse)
    {
        await _warehouseRepository.AddAsync(warehouse);
        await _unitOfWork.SaveChangesAsync();

        return warehouse;
    }

    public async Task<Warehouse?> UpdateAsync(int id, Warehouse warehouse)
    {
        var existing = await _warehouseRepository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Name = warehouse.Name;
        existing.Location = warehouse.Location;


        _warehouseRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _warehouseRepository.GetByIdAsync(id);
        if (existing == null) return false;

        _warehouseRepository.Delete(existing);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
