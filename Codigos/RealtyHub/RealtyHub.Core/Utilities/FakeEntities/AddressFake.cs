using Bogus;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Endereco"/></c>.
/// </summary>
public class AddressFake
{
    /// <summary>
    /// Gera um endereço falso para a entidade <c><see cref="Endereco"/></c> utilizando a biblioteca Bogus.
    /// </summary>
    /// <returns>Um objeto <c><see cref="Endereco"/></c> com dados gerados aleatoriamente.</returns>
    public static Endereco GetFakeAddress()
    {
        return new Faker<Endereco>(Configuration.Locale)
            .RuleFor(a => a.Logradouro, a => a.Address.StreetName())
            .RuleFor(a => a.Bairro, a => a.Address.SecondaryAddress())
            .RuleFor(a => a.Numero, a => a.Address.BuildingNumber())
            .RuleFor(a => a.Cidade, a => a.Address.City())
            .RuleFor(a => a.Estado, a => a.Address.State())
            .RuleFor(a => a.Pais, a => a.Address.Country())
            .RuleFor(a => a.Cep, a => a.Address.ZipCode("#####-###"))
            .RuleFor(a => a.Complemento, a => a.Address.SecondaryAddress());
    }
}