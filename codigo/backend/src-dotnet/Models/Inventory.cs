using System.ComponentModel.DataAnnotations;
namespace Backend.Models;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    public required string OfficeNum { get; set; }
    public required string Sku { get; set; }
    public required int Quantity { get; set; }
}

public class CartItem
{
    public required string Sku { get; set; }
    public required string OfficeNum { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public required int Quantity { get; set; }
}

public class CartDto
{
    public required List<CartItem> Items { get; set; }
}
