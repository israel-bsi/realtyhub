using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Address
{
    [Required(ErrorMessage = "Logradouro é um campo obrigatório")]
    public string Street { get; set; } = string.Empty;
    [Required(ErrorMessage = "Bairro é um campo obrigatório")]
    public string Neighborhood { get; set; } = string.Empty;
    [Required(ErrorMessage = "Número é um campo obrigatório")]
    public string Number { get; set; } = string.Empty;
    [Required(ErrorMessage = "Cidade é um campo obrigatório")]
    public string City { get; set; } = string.Empty;
    [Required(ErrorMessage = "Estado é um campo obrigatório")]
    public string State { get; set; } = string.Empty;
    [Required(ErrorMessage = "País é um campo obrigatório")]
    public string Country { get; set; } = string.Empty;
    [Required(ErrorMessage = "Cep é um campo obrigatório")]
    public string ZipCode { get; set; } = string.Empty;
    public string? Complement { get; set; }
}