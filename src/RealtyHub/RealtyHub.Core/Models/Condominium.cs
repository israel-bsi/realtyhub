using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Condominium : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Nome é um campo obrigatório")]
    [MaxLength(120, ErrorMessage = "O Nome deve conter até 120 caracteres")]
    public string Name { get; set; } = string.Empty;
    [ValidateComplexType]
    public Address Address { get; set; } = new();
    public int Units { get; set; }
    public int Floors { get; set; }
    public bool HasElevator { get; set; }
    public bool HasSwimmingPool { get; set; }
    public bool HasPartyRoom { get; set; }
    public bool HasPlayground { get; set; }
    public bool HasFitnessRoom { get; set; }
    public decimal CondominiumValue { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ICollection<Property> Properties { get; set; } = [];
}