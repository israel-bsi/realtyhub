using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.Properties;

public class CreatePropertyRequest : Request
{
    [Required(ErrorMessage = "Título é um campo obrigatório")]
    [MaxLength(120, ErrorMessage = "A Título deve conter até 120 caracteres")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Descrição é um campo obrigatório")]
    [MaxLength(80, ErrorMessage = "A descrição deve conter até 80 caracteres")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Preço é um campo obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço inválido")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Tipo de propriedade é um campo obrigatório")]
    public EPropertyType PropertyType { get; set; }
    [Required(ErrorMessage = "Quartos é um campo obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "Quartos inválidos")]
    public int Bedroom { get; set; }
    [Required(ErrorMessage = "Banheiros é um campo obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "Banheiros inválidos")]
    public int Bathroom { get; set; }
    [Required(ErrorMessage = "Garagem é um campo obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "Garagem inválida")]
    public int Garage { get; set; }
    [Required(ErrorMessage = "Área é um campo obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "Área inválida")]
    public double Area { get; set; }
    [Required(ErrorMessage = "Detalhes de transações é um campo obrigatório")]
    public string TransactionsDetails { get; set; } = string.Empty;
    [Required(ErrorMessage = "Endereço é um campo obrigatório")]
    public Address Address { get; set; } = new();
    [JsonIgnore]
    public List<PropertyImage> PropertyImage { get; set; } = [];
    [Required(ErrorMessage = "Propriedade nova é um campo obrigatório")]
    public bool IsNew { get; set; }
}