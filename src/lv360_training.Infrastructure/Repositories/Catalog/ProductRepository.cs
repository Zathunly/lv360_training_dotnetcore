using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories.Catalog;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.ToListAsync();

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public void Update(Product product) =>
        _context.Products.Update(product);

    public void Delete(Product product) =>
        _context.Products.Remove(product);
}
