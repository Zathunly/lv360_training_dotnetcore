using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Entities;

namespace lv360_training.Application.Handlers;

public class CategoryHandler
{
    private readonly IBasedCatalogRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryHandler(IBasedCatalogRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Category?> GetByIdAsync(int id) =>
        await _categoryRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _categoryRepository.GetAllAsync();

    public async Task<Category> CreateAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        var existing = await _categoryRepository.GetByIdAsync(category.Id);
        if (existing == null) return null;

        existing.Name = category.Name;

        _categoryRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null) return false;

        _categoryRepository.Delete(existing);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
