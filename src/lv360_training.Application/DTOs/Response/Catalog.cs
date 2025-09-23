
using System.ComponentModel.DataAnnotations;
using lv360_training.Domain.Entities;

namespace lv360_training.Application.Dtos.Catalog.Response;

public class OrderSuccessDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
