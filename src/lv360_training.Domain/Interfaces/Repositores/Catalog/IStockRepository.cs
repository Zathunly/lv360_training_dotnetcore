using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Catalog
{
    public interface IStockRepository : IBasedCatalogRepository<Stock>
    {
        Task<Stock?> GetByProductIdAsync(int productId);
        Task<Stock?> GetByProductAndWarehouseAsync(int productId, int warehouseId);
    }
}
