using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Customer : Entity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public EDocumentType DocumentType { get; set; }
    public ECustomerType CustomerType => DocumentType switch
    {
        EDocumentType.Cpf => ECustomerType.Individual,
        EDocumentType.Cnpj => ECustomerType.Business,
        _ => ECustomerType.Individual
    };
    public Address Address { get; set; } = new();
    public string? Rg { get; set; }
    public string? BusinessName { get; set; }
    public bool IsActive { get; set; }
}