using System.ComponentModel.DataAnnotations;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.Properties;

public class UpdatePropertyRequest : Request
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Título é um campo obrigatório")]
    [MaxLength(120, ErrorMessage = "A Título deve conter até 120 caracteres")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Descrição é um campo obrigatório")]
    [MaxLength(80, ErrorMessage = "A descrição deve conter até 80 caracteres")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Preço é um campo obrigatório")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Tipo de propriedade é um campo obrigatório")]
    public EPropertyType PropertyType { get; set; }
    [Required(ErrorMessage = "Quartos é um campo obrigatório")]
    public int Bedroom { get; set; }
    [Required(ErrorMessage = "Banheiros é um campo obrigatório")]
    public int Bathroom { get; set; }
    [Required(ErrorMessage = "Garagem é um campo obrigatório")]
    public int Garage { get; set; }
    [Required(ErrorMessage = "Área é um campo obrigatório")]
    public double Area { get; set; }
    [Required(ErrorMessage = "Detalhes de transações é um campo obrigatório")]
    public string TransactionsDetails { get; set; } = string.Empty;
    [Required(ErrorMessage = "Endereço é um campo obrigatório")]
    public Address Address { get; set; } = new();
    public List<PropertyImage> PropertyImage { get; set; } = [];
    [Required(ErrorMessage = "Propriedade nova é um campo obrigatório")]
    public bool IsNew { get; set; }

    public static implicit operator UpdatePropertyRequest(Property request) =>
        new()
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            PropertyType = request.PropertyType,
            Bedroom = request.Bedroom,
            Bathroom = request.Bathroom,
            Garage = request.Garage,
            Area = request.Area,
            TransactionsDetails = request.TransactionsDetails,
            Address = request.Address,
            PropertyImage = request.PropertyImages,
            IsNew = request.IsNew
        };
}