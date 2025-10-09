using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Entities;

namespace lv360_training.Application.Handlers;

public class ProductHandler
{
    private readonly IBasedCatalogRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductHandler(IBasedCatalogRepository<Product> productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _productRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _productRepository.GetAllAsync();

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var existing = await _productRepository.GetByIdAsync(product.Id);
        if (existing == null) return null;

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.UpdatedAt = DateTime.UtcNow;

        _productRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null) return false;

        _productRepository.Delete(existing);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Product>> CreateBulkAsync(IEnumerable<Product> products)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var p in products)
        {
            p.CreatedAt = utcNow;
            p.UpdatedAt = utcNow;
        }

        await _productRepository.AddRangeAsync(products);
        await _unitOfWork.SaveChangesAsync();

        return products;
    }

    public async Task<IEnumerable<Product>> UpdateBulkAsync(IEnumerable<Product> products)
    {
        var ids = products.Select(p => p.Id).ToList();
        var existingProducts = (await _productRepository.GetAllAsync())
            .Where(p => ids.Contains(p.Id))
            .ToDictionary(p => p.Id);

        var utcNow = DateTime.UtcNow;
        var toUpdate = new List<Product>();

        foreach (var product in products)
        {
            if (existingProducts.TryGetValue(product.Id, out var existing))
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.UpdatedAt = utcNow;

                toUpdate.Add(existing);
            }
        }

        _productRepository.UpdateRange(toUpdate);
        await _unitOfWork.SaveChangesAsync();

        return toUpdate;
    }

    public async Task<int> DeleteBulkAsync(IEnumerable<int> ids)
    {
        var products = (await _productRepository.GetAllAsync())
            .Where(p => ids.Contains(p.Id))
            .ToList();

        if (!products.Any()) return 0;

        _productRepository.DeleteRange(products);
        await _unitOfWork.SaveChangesAsync();

        return products.Count;
    }
}
