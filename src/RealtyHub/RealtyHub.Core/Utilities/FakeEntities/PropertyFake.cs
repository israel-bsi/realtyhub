using Bogus;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

public class PropertyFake
{
    public static List<Property> GetFakeProperties(int quantity)
    {
        var propertyFake = new Faker<Property>(Configuration.Locale)
            .RuleFor(p => p.Title, p => p.Commerce.ProductName())
            .RuleFor(p => p.Description, p => p.Commerce.ProductDescription())
            .RuleFor(p => p.Price, p => p.Random.Decimal(100000, 1000000))
            .RuleFor(p => p.PropertyType, p => p.PickRandom<EPropertyType>())
            .RuleFor(p => p.Bedroom, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Bathroom, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Garage, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Area, p => p.Random.Double(50, 500))
            .RuleFor(p => p.TransactionsDetails, p => p.Commerce.ProductMaterial())
            .RuleFor(p => p.Address, AddressFake.GetFakeAddress)
            .RuleFor(p => p.IsNew, true)
            .RuleFor(p => p.ShowInHome, true)
            .RuleFor(p => p.IsActive, true);

        return propertyFake.Generate(quantity);
    }
}