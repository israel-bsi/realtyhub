using Bogus;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Property"/></c>.
/// </summary>
public class PropertyFake
{
    /// <summary>
    /// Gera uma lista de imóveis falsas.
    /// </summary>
    /// <param name="quantity">A quantidade de imóveis a serem geradas.</param>
    /// <param name="customerId">O ID do cliente (vendedor) associado a cada propriedade.</param>
    /// <param name="condominiumId">O ID do condomínio associado a cada propriedade.</param>
    /// <returns>Uma lista de objetos <c><see cref="Property"/></c> com dados gerados aleatoriamente.</returns>
    public static List<Property> GetFakeProperties(int quantity, int customerId, int condominiumId)
    {
        var propertyFake = new Faker<Property>(Configuration.Locale)
            .RuleFor(p => p.Title, p => p.Commerce.ProductName())
            .RuleFor(p => p.Description, p => p.Commerce.ProductDescription())
            .RuleFor(p => p.Price, p => decimal.Parse(p.Commerce.Price()))
            .RuleFor(p => p.PropertyType, p => p.PickRandom<EPropertyType>())
            .RuleFor(p => p.Bedroom, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Bathroom, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Garage, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Area, p => double.Parse(p.Random.Double(50, 500).ToString("F2")))
            .RuleFor(p => p.TransactionsDetails, p => p.Commerce.ProductMaterial())
            .RuleFor(p => p.Address, AddressFake.GetFakeAddress)
            .RuleFor(p => p.SellerId, customerId)
            .RuleFor(p => p.CondominiumId, condominiumId)
            .RuleFor(p => p.RegistryNumber, p => p.Random.UInt().ToString())
            .RuleFor(p => p.RegistryRecord, p => p.Random.UInt().ToString())
            .RuleFor(p => p.IsNew, true)
            .RuleFor(p => p.ShowInHome, true)
            .RuleFor(p => p.IsActive, true);

        return propertyFake.Generate(quantity);
    }
}