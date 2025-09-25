using System.ComponentModel.DataAnnotations;
using lv360_training.Domain.Entities;

namespace lv360_training.Application.Dtos.Catalog.Request;

// Product
public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }
}

// Stock
public class CreateOrUpdateStockDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrUpdateCategory
{
    public string Name { get; set; } = string.Empty;
}
//Order 
/// </summary>
public class PlaceOrderRequestDto
{
    public List<ProductOrderItem> Items { get; set; } = new List<ProductOrderItem>();

    public class ProductOrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

public class UpdateOrderRequestDto
{
    public OrderStatus Status { get; set; }

    public List<OrderItemUpdateDto> Items { get; set; } = new();

    public class OrderItemUpdateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}


// Warehouse
public class CreateOrUpdateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
}
