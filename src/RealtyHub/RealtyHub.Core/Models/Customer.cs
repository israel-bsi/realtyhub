using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Customer : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Nome é um campo obrigatório")]
    [MaxLength(80, ErrorMessage = "O nome deve conter até 80 caracteres")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email é um campo obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(50, ErrorMessage = "O e-mail deve conter até 80 caracteres")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Telefone é um campo obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    [MaxLength(30, ErrorMessage = "O telefone deve conter até 80 caracteres")]
    public string Phone { get; set; } = string.Empty;
    [Required(ErrorMessage = "Documento é um campo obrigatório")]
    [MaxLength(20, ErrorMessage = "O documento deve conter até 20 caracteres")]
    public string DocumentNumber { get; set; } = string.Empty;
    public EDocumentType DocumentType { get; set; } = EDocumentType.Cpf;
    [Required(ErrorMessage = "Tipo de cliente é um campo obrigatório")]
    public ECustomerType CustomerType { get; set; } = ECustomerType.Individual;
    [ValidateComplexType]
    public Address Address { get; set; } = new();
    public string? Rg { get; set; }
    public string? BusinessName { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
}