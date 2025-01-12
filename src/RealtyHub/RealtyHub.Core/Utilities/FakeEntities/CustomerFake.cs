using Bogus;
using Bogus.Extensions.Brazil;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

public class CustomerFake
{
    private const string Locale = "pt_BR";
    public static List<Customer> GetFakeBusinessCustomers(int quantity)
    {
        var businessCustomerFake = new Faker<Customer>(Locale)
            .RuleFor(c => c.Name, c => c.Company.CompanyName())
            .RuleFor(c => c.Email, c=> c.Person.Email)
            .RuleFor(c => c.Phone, c=> c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Company.Cnpj(false))
            .RuleFor(c => c.CustomerType, ECustomerType.Business)
            .RuleFor(c => c.DocumentType, EDocumentType.Cnpj)
            .RuleFor(c => c.Address, GetFakeAddress)
            .RuleFor(c => c.BusinessName, c => c.Company.CompanyName())
            .RuleFor(c => c.IsActive, true);

        return businessCustomerFake.Generate(quantity);
    }
    public static List<Customer> GetFakeIndividualCustomers(int quantity)
    {
        var individualCustomerFake = new Faker<Customer>(Locale)
            .RuleFor(c => c.Name, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Person.Cpf(false))
            .RuleFor(c => c.DocumentType, EDocumentType.Cpf)
            .RuleFor(c => c.CustomerType, ECustomerType.Individual)
            .RuleFor(c => c.Address, GetFakeAddress)
            .RuleFor(c => c.Rg, c => c.Person.Cpf(false))
            .RuleFor(c => c.IsActive, true);

        return individualCustomerFake.Generate(quantity);
    }
    private static Address GetFakeAddress()
    {
        return new Faker<Address>(Locale)
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