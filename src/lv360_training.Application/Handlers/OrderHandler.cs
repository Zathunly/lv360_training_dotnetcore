using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Repositories.Catalog;
using lv360_training.Domain.Interfaces.Repositories.Core;
namespace lv360_training.Application.Handlers;

public class OrderHandler
{
    private readonly IBasedCatalogRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderHandler(IBasedCatalogRepository<Order> orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> PlaceOrder(Order order)
    {
        ValidateOrderItems(order);

        // Add order
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Reload order including Products for DTO mapping
        var createdOrder = await _orderRepository.GetByIdAsync(order.Id);
        if (createdOrder == null)
            throw new InvalidOperationException("Order not found after saving.");

        return createdOrder;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        ValidateOrderItems(order);

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return order;
    }

    public async Task<bool> Delete(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return false;

        _orderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // --- Private Helper ---
    /// <summary>
    /// Validate order items: Quantity must be > 0
    /// </summary>
    private void ValidateOrderItems(Order order)
    {
        foreach (var item in order.OrderItems)
        {
            if (item.Quantity <= 0)
                throw new InvalidOperationException(
                    $"Order item for product {item.ProductId} has invalid quantity {item.Quantity}."
                );
        }
    }
}
