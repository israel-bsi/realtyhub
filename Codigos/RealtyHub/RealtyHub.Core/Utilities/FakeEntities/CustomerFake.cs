using Bogus;
using Bogus.Extensions.Brazil;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

/// <summary>
/// Fornece métodos para gerar dados falsos para a entidade <c><see cref="Customer"/></c>.
/// </summary>
public class CustomerFake
{
    /// <summary>
    /// Gera uma lista de clientes empresariais falsos.
    /// </summary>
    /// <param name="quantity">A quantidade de clientes a serem gerados.</param>
    /// <returns>Uma lista de objetos <c><see cref="Customer"/></c> com dados gerados aleatoriamente para clientes empresariais.</returns>
    public static List<Customer> GetFakeBusinessCustomers(int quantity)
    {
        var businessCustomerFake = new Faker<Customer>(Configuration.Locale)
            .RuleFor(c => c.Name, c => c.Company.CompanyName())
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Company.Cnpj(false))
            .RuleFor(c => c.PersonType, EPersonType.Business)
            .RuleFor(c => c.CustomerType, ECustomerType.Seller)
            .RuleFor(c => c.Address, AddressFake.GetFakeAddress)
            .RuleFor(c => c.BusinessName, c => c.Company.CompanyName())
            .RuleFor(c => c.IsActive, true);

        return businessCustomerFake.Generate(quantity);
    }

    /// <summary>
    /// Gera uma lista de clientes individuais falsos.
    /// </summary>
    /// <param name="quantity">A quantidade de clientes a serem gerados.</param>
    /// <returns>Uma lista de objetos <c><see cref="Customer"/></c> com dados gerados aleatoriamente para clientes individuais.</returns>
    public static List<Customer> GetFakeIndividualCustomers(int quantity)
    {
        var individualCustomerFake = new Faker<Customer>(Configuration.Locale)
            .RuleFor(c => c.Name, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Person.Cpf(false))
            .RuleFor(c => c.PersonType, EPersonType.Individual)
            .RuleFor(c => c.CustomerType, ECustomerType.Seller)
            .RuleFor(c => c.Address, AddressFake.GetFakeAddress)
            .RuleFor(c => c.Rg, c => c.Person.Cpf(false))
            .RuleFor(c => c.IssuingAuthority, c => c.Address.City())
            .RuleFor(c => c.RgIssueDate, c => c.Date.Past().ToUniversalTime())
            .RuleFor(c => c.MaritalStatus, c => c.Random.Enum<EMaritalStatus>())
            .RuleFor(c => c.Occupation, c => c.Company.CompanyName())
            .RuleFor(c => c.Nationality, c => c.Address.Country())
            .RuleFor(c => c.IsActive, true);

        return individualCustomerFake.Generate(quantity);
    }
}