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
}
