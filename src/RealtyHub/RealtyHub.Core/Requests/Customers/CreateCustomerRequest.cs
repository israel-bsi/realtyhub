using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.Customers;

public class CreateCustomerRequest : Request
{
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
    public Address Address { get; set; } = new();
    [Required(ErrorMessage = "Documento é um campo obrigatório")]
    [MaxLength(20, ErrorMessage = "O documento deve conter até 20 caracteres")]
    public string DocumentNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Tipo de cliente é um campo obrigatório")]
    public ECustomerType CustomerType { get; set; }
    public string? Rg { get; set; }
    public string? BusinessName { get; set; }
}