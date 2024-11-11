using System.ComponentModel.DataAnnotations;
namespace Backend.Models;

public class AvailableCeps
{
    public required int Id { get; set; }

    public required string OfficeNum { get; set; }
    public required string Cep { get; set; }
}
