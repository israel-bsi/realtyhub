using Bogus;
using Bogus.Extensions.Brazil;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Cliente"/></c>.
/// </summary>
public class CustomerFake
{
    /// <summary>
    /// Gera uma lista de clientes empresariais falsos.
    /// </summary>
    /// <param name="quantity">A quantidade de clientes a serem gerados.</param>
    /// <returns>Uma lista de objetos <c><see cref="Cliente"/></c> com dados gerados aleatoriamente para clientes empresariais.</returns>
    public static List<Cliente> GetFakeBusinessCustomers(int quantity)
    {
        var businessCustomerFake = new Faker<Cliente>(Configuration.Locale)
            .RuleFor(c => c.Nome, c => c.Company.CompanyName())
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Telefone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.NumeroDocumento, c => c.Company.Cnpj(false))
            .RuleFor(c => c.TipoPessoa, ETipoPessoa.Juridica)
            .RuleFor(c => c.TipoCliente, ETipoCliente.Vendedor)
            .RuleFor(c => c.Endereco, AddressFake.GetFakeAddress)
            .RuleFor(c => c.NomeFantasia, c => c.Company.CompanyName())
            .RuleFor(c => c.Ativo, true);

        return businessCustomerFake.Generate(quantity);
    }

    /// <summary>
    /// Gera uma lista de clientes individuais falsos.
    /// </summary>
    /// <param name="quantity">A quantidade de clientes a serem gerados.</param>
    /// <returns>Uma lista de objetos <c><see cref="Cliente"/></c> com dados gerados aleatoriamente para clientes individuais.</returns>
    public static List<Cliente> GetFakeIndividualCustomers(int quantity)
    {
        var individualCustomerFake = new Faker<Cliente>(Configuration.Locale)
            .RuleFor(c => c.Nome, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Telefone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.NumeroDocumento, c => c.Person.Cpf(false))
            .RuleFor(c => c.TipoPessoa, ETipoPessoa.Fisica)
            .RuleFor(c => c.TipoCliente, ETipoCliente.Vendedor)
            .RuleFor(c => c.Endereco, AddressFake.GetFakeAddress)
            .RuleFor(c => c.Rg, c => c.Person.Cpf(false))
            .RuleFor(c => c.AutoridadeEmissora, c => c.Address.City())
            .RuleFor(c => c.DataEmissaoRg, c => c.Date.Past().ToUniversalTime())
            .RuleFor(c => c.TipoStatusCivil, c => c.Random.Enum<ETipoStatusCivil>())
            .RuleFor(c => c.Ocupacao, c => c.Company.CompanyName())
            .RuleFor(c => c.Nacionalidade, c => c.Address.Country())
            .RuleFor(c => c.Ativo, true);

        return individualCustomerFake.Generate(quantity);
    }
}