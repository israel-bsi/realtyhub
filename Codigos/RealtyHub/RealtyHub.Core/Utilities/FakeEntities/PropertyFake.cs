using Bogus;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Imovel"/></c>.
/// </summary>
public class PropertyFake
{
    /// <summary>
    /// Gera uma lista de imóveis falsas.
    /// </summary>
    /// <param name="quantity">A quantidade de imóveis a serem geradas.</param>
    /// <param name="customerId">O ID do cliente (vendedor) associado a cada propriedade.</param>
    /// <param name="condominiumId">O ID do condomínio associado a cada propriedade.</param>
    /// <returns>Uma lista de objetos <c><see cref="Imovel"/></c> com dados gerados aleatoriamente.</returns>
    public static List<Imovel> GetFakeProperties(int quantity, int customerId, int condominiumId)
    {
        var propertyFake = new Faker<Imovel>(Configuration.Locale)
            .RuleFor(p => p.Titulo, p => p.Commerce.ProductName())
            .RuleFor(p => p.Descricao, p => p.Commerce.ProductDescription())
            .RuleFor(p => p.Preco, p => decimal.Parse(p.Commerce.Price()))
            .RuleFor(p => p.TipoImovel, p => p.PickRandom<ETipoImovel>())
            .RuleFor(p => p.Quarto, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Banheiro, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Garagem, p => p.Random.Int(1, 5))
            .RuleFor(p => p.Area, p => double.Parse(p.Random.Double(50, 500).ToString("F2")))
            .RuleFor(p => p.DetalhesTransacao, p => p.Commerce.ProductMaterial())
            .RuleFor(p => p.Endereco, AddressFake.GetFakeAddress)
            .RuleFor(p => p.VendedorId, customerId)
            .RuleFor(p => p.CondominioId, condominiumId)
            .RuleFor(p => p.NumeroRegistro, p => p.Random.UInt().ToString())
            .RuleFor(p => p.RegistroCartorio, p => p.Random.UInt().ToString())
            .RuleFor(p => p.Novo, true)
            .RuleFor(p => p.ExibirNaHome, true)
            .RuleFor(p => p.Ativo, true);

        return propertyFake.Generate(quantity);
    }
}