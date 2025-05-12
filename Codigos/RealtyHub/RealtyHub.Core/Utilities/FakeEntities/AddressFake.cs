using Bogus;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Address"/></c>.
/// </summary>
public class AddressFake
{
    /// <summary>
    /// Gera um endereço falso para a entidade <c><see cref="Address"/></c> utilizando a biblioteca Bogus.
    /// </summary>
    /// <returns>Um objeto <c><see cref="Address"/></c> com dados gerados aleatoriamente.</returns>
    public static Address GetFakeAddress()
    {
        return new Faker<Address>(Configuration.Locale)
            .RuleFor(a => a.Street, a => a.Address.StreetName())
            .RuleFor(a => a.Neighborhood, a => a.Address.SecondaryAddress())
            .RuleFor(a => a.Number, a => a.Address.BuildingNumber())
            .RuleFor(a => a.City, a => a.Address.City())
            .RuleFor(a => a.State, a => a.Address.State())
            .RuleFor(a => a.Country, a => a.Address.Country())
            .RuleFor(a => a.ZipCode, a => a.Address.ZipCode("#####-###"))
            .RuleFor(a => a.Complement, a => a.Address.SecondaryAddress());
    }
}