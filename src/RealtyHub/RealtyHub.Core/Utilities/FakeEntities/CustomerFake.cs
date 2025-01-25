using Bogus;
using Bogus.Extensions.Brazil;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Utilities.FakeEntities;

public class CustomerFake
{
    public static List<Customer> GetFakeBusinessCustomers(int quantity)
    {
        var businessCustomerFake = new Faker<Customer>(Configuration.Locale)
            .RuleFor(c => c.Name, c => c.Company.CompanyName())
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Company.Cnpj(false))
            .RuleFor(c => c.CustomerType, ECustomerType.Business)
            .RuleFor(c => c.Address, AddressFake.GetFakeAddress)
            .RuleFor(c => c.BusinessName, c => c.Company.CompanyName())
            .RuleFor(c => c.IsActive, true);

        return businessCustomerFake.Generate(quantity);
    }
    public static List<Customer> GetFakeIndividualCustomers(int quantity)
    {
        var individualCustomerFake = new Faker<Customer>(Configuration.Locale)
            .RuleFor(c => c.Name, c => c.Person.FullName)
            .RuleFor(c => c.Email, c => c.Person.Email)
            .RuleFor(c => c.Phone, c => c.Phone.PhoneNumber("###########"))
            .RuleFor(c => c.DocumentNumber, c => c.Person.Cpf(false))
            .RuleFor(c => c.CustomerType, ECustomerType.Individual)
            .RuleFor(c => c.Address, AddressFake.GetFakeAddress)
            .RuleFor(c => c.Rg, c => c.Person.Cpf(false))
            .RuleFor(c=>c.MaritalStatus, c=>c.Random.Enum<EMaritalStatus>())
            .RuleFor(c=>c.Occupation, c=>c.Company.CompanyName())
            .RuleFor(c=>c.Nationality, c=>c.Address.Country())
            .RuleFor(c => c.IsActive, true);

        return individualCustomerFake.Generate(quantity);
    }

}