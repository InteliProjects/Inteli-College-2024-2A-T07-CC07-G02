using System.ComponentModel.DataAnnotations;
namespace Backend.Models;


public class Products
{
    [Key]
    public required string Sku { get; set; }

    public string? ProductName { get; set; }
    public string? ImagePath { get; set; }
}
