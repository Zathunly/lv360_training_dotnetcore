using System.ComponentModel.DataAnnotations;

namespace lv360_training.Application.Dtos.Catalog;

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

public class CreateStockDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
}
