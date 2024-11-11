using System.ComponentModel.DataAnnotations;
namespace Backend.Models;

public class Stores
{
    [Key]
    public required string OfficeNum { get; set; }

    public required string Cep { get; set; }
    public required string Status { get; set; }
    public required string DeliveryTime { get; set; }
}
