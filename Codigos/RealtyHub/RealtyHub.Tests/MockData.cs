using RealtyHub.Core.Enums;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests;

public static class MockData
{
    public static Condominium GetValidCondominium()
    {
        return new Condominium
        {
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Rua do Condomínio",
                Number = "789",
                Neighborhood = "Vila Nova",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            Units = 50,
            Floors = 10,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = false,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 350.50m,
            UserId = RealtyHubApiTests.TestUserId,
            IsActive = true
        };
    }

    public static Customer GetValidCustomer(ECustomerType customerType)
    {
        return new Customer
        {
            Name = $"{customerType.GetDisplayName()} Teste",
            Email = $"{customerType.GetDisplayName().ToLower()}@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = customerType,
            PersonType = EPersonType.Individual,
            Occupation = $"{customerType.GetDisplayName()}",
            Nationality = "Brasileira",
            MaritalStatus = EMaritalStatus.Single,
            UserId = RealtyHubApiTests.TestUserId,
            Address = new Address
            {
                Street = $"Rua do {customerType.GetDisplayName()}",
                Number = "456",
                Neighborhood = "Jardins",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            },
            IsActive = true
        };
    }

    public static Property GetValidProperty(Customer seller, Condominium condominium)
    {
        return new Property
        {
            Title = "Imóvel Teste",
            Description = "Descrição do imóvel teste",
            Price = 500000.00m,
            PropertyType = EPropertyType.Apartment,
            Bedroom = 3,
            Bathroom = 2,
            Garage = 1,
            Area = 120.5,
            TransactionsDetails = "Detalhes da transação",
            SellerId = seller.Id,
            CondominiumId = condominium.Id,
            RegistryNumber = "123456789",
            RegistryRecord = "987654321",
            IsNew = true,
            ShowInHome = true,
            IsActive = true,
            UserId = RealtyHubApiTests.TestUserId,
            Address = new Address
            {
                Street = "Rua do Imóvel",
                Number = "999",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234567"
            }
        };
    }

    public static Offer GetValidOffer(Customer buyer, Property property)
    {
        var payments = new List<Payment>
        {
            new Payment
            {
                Amount = decimal.Parse(Random.Shared.NextInt64(100, 100000).ToString()),
                PaymentType = EPaymentType.Pix,
                UserId = RealtyHubApiTests.TestUserId,
                IsActive = true
            },
            new Payment
            {
                Amount = decimal.Parse(Random.Shared.NextInt64(100, 100000).ToString()),
                PaymentType = EPaymentType.Cash,
                UserId = RealtyHubApiTests.TestUserId,
                IsActive = true
            }
        };
        return new Offer
        {
            Amount = payments.Sum(c => c.Amount),
            SubmissionDate = DateTime.Now.AddDays(-30),
            OfferStatus = EOfferStatus.Accepted,
            BuyerId = buyer.Id,
            PropertyId = property.Id,
            UserId = RealtyHubApiTests.TestUserId,
            
            Payments = payments
        };
    }

    public static Contract GetValidContract(Offer offer, Customer seller, Customer buyer)
    {
        return new Contract
        {
            SellerId = seller.Id,
            Seller = seller,
            BuyerId = buyer.Id,
            Buyer = buyer,
            OfferId = offer.Id,
            Offer = offer,
            IssueDate = DateTime.Now,
            EffectiveDate = DateTime.Now.AddDays(30),
            TermEndDate = DateTime.Now.AddYears(1),
            SignatureDate = DateTime.Now.AddDays(7),
            FileId = Guid.NewGuid().ToString(),
            IsActive = true,
            UserId = RealtyHubApiTests.TestUserId
        };
    }
}