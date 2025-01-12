using Bogus;
using Bogus.Extensions.Brazil;
using Bogus.Extensions.Romania;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests.FakeEntities;

public class CustomerFake
{
    public static List<Customer> GetFakeBusinessCustomers(int quantity)
    {
        var businessCustomerFake = new Faker<Customer>()
            .RuleFor(c => c.Id, c =>c.UniqueIndex)
            .RuleFor(c => c.Name, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Person.Phone)
            .RuleFor(c => c.DocumentNumber, c => c.Person.Cnp())
            .RuleFor(c => c.DocumentType, c => EDocumentType.Cnpj)
            .RuleFor(c => c.Address, GetFakeAddress)
            .RuleFor(c => c.BusinessName, c => c.Company.CompanyName());

        return businessCustomerFake.Generate(quantity);
    }
    public static List<Customer> GetFakeIndividualCustomers(int quantity)
    {
        var individualCustomerFake = new Faker<Customer>()
            .RuleFor(c => c.Id, c => c.UniqueIndex)
            .RuleFor(c => c.Name, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Person.Phone)
            .RuleFor(c => c.DocumentNumber, c => c.Person.Cpf())
            .RuleFor(c => c.DocumentType, c => EDocumentType.Cpf)
            .RuleFor(c => c.Address, GetFakeAddress)
            .RuleFor(c => c.Rg, c => c.Random.String(11));

        return individualCustomerFake.Generate(quantity);
    }
    private static Address GetFakeAddress()
    {
        return new Faker<Address>()
            .RuleFor(a => a.Number, a => a.Random.Number(1, 999).ToString())
            .RuleFor(a => a.Street, a => a.Address.StreetName())
            .RuleFor(a => a.Complement, a => a.Address.SecondaryAddress())
            .RuleFor(a => a.Neighborhood, a => a.Address.County())
            .RuleFor(a => a.City, a => a.Address.City())
            .RuleFor(a => a.State, a => a.Address.State())
            .RuleFor(a => a.ZipCode, a => a.Address.ZipCode("#####-###"));
    }
}