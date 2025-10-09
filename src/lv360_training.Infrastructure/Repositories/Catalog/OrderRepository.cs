using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories;

public class OrderRepository : IBasedCatalogRepository<Order>
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order entity)
    {
        ValidateOrderItems(entity);

        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Order entity)
    {
        ValidateOrderItems(entity);

        _context.Orders.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(Order entity)
    {
        _context.Orders.Remove(entity);
        _context.SaveChanges();
    }

    public async Task AddRangeAsync(IEnumerable<Order> entities)
    {
        foreach (var order in entities)
            ValidateOrderItems(order);

        await _context.Orders.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public void UpdateRange(IEnumerable<Order> entities)
    {
        foreach (var order in entities)
            ValidateOrderItems(order);

        _context.Orders.UpdateRange(entities);
        _context.SaveChanges();
    }

    public void DeleteRange(IEnumerable<Order> entities)
    {
        _context.Orders.RemoveRange(entities);
        _context.SaveChanges();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
    }

    /// <summary>
    /// Validates order items: Quantity must be > 0.
    /// Throws InvalidOperationException if invalid.
    /// </summary>
    private void ValidateOrderItems(Order order)
    {
        foreach (var item in order.OrderItems)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException(
                    $"Order item for product {item.ProductId} has invalid quantity {item.Quantity}."
                );
            }
        }
    }
}
