using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um endereço no sistema.
/// </summary>
public class Address
{
    /// <summary>
    /// Logradouro do endereço.
    /// </summary>
    /// <value>O logradouro do endereço.</value>
    [Required(ErrorMessage = "Logradouro é um campo obrigatório")]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Bairro do endereço.
    /// </summary>
    /// <value>O bairro do endereço.</value>
    [Required(ErrorMessage = "Bairro é um campo obrigatório")]
    public string Neighborhood { get; set; } = string.Empty;

    /// <summary>
    /// Número do endereço.
    /// </summary>
    /// <value>O número do endereço.</value>
    [Required(ErrorMessage = "Número é um campo obrigatório")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Cidade do endereço.
    /// </summary>
    /// <value>O nome da cidade do endereço.</value>
    [Required(ErrorMessage = "Cidade é um campo obrigatório")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Estado do endereço.
    /// </summary>
    /// <value>O nome do estado do endereço.</value>  
    [Required(ErrorMessage = "Estado é um campo obrigatório")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// País do endereço.
    /// </summary>
    /// <value>O nome do país do endereço.</value>
    [Required(ErrorMessage = "País é um campo obrigatório")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Código postal (CEP) do endereço.
    /// </summary>
    /// <value>O código postal do endereço.</value>
    [Required(ErrorMessage = "Cep é um campo obrigatório")]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Complemento do endereço.
    /// </summary>
    /// <value>Informações adicionais sobre o endereço.</value>
    public string Complement { get; set; } = string.Empty;
}