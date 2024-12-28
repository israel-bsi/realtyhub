using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class Property : Entity
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public EPropertyType PropertyType { get; set; }
    public int Bedroom { get; set; }
    public int Bathroom { get; set; }
    public int Garage { get; set; }
    public double Area { get; set; }
    public string TransactionsDetails { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public List<PropertyImage> PropertyImage { get; set; } = [];
    public Address Address { get; set; } = new();
}