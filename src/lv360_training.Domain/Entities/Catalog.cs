using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lv360_training.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    // Relationships
    public int CategoryId { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Stock
{
    public int Id { get; set; }

    // FK → Product
    [JsonIgnore]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // FK → Warehouse
    [JsonIgnore]
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;   
    public string? Location { get; set; }              

    [JsonIgnore]
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}


public class Order
{
    public int Id { get; set; }

    // Foreign Key
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore] 
    public User User { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Computed property, Won't be created in Database
    [NotMapped]
    public decimal TotalAmount => OrderItems.Sum(i => i.UnitPrice * i.Quantity);

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }

    [JsonIgnore]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [JsonIgnore]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal SubTotal => Quantity * UnitPrice;
}

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Completed,
    Cancelled
}
