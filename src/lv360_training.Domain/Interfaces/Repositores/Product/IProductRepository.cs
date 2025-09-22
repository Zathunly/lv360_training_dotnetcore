namespace lv360_training.Domain.Interfaces.Repositories.Catalog;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}
