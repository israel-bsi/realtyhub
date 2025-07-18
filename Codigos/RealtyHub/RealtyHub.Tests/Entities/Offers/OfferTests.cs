using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests.Entities.Offers;

public class OfferTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private async Task<(Property property, Customer customer)> SeedTestData(AppDbContext context)
    {
        var property = new Property
        {
            Title = "Casa de Teste",
            Description = "Descrição da casa",
            Price = 500000,
            UserId = "seller123",
            IsActive = true,
            PropertyType = EPropertyType.House,
            Bedroom = 3,
            Bathroom = 2,
            Area = 150,
            Garage = 2,
            IsNew = true,
            RegistryNumber = "12345",
            RegistryRecord = "Livro 1",
            TransactionsDetails = "Detalhes da transação",
            ShowInHome = true,
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        var customer = new Customer
        {
            Name = "João Silva",
            Email = "joao@teste.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = ECustomerType.Buyer,
            PersonType = EPersonType.Individual,
            IsActive = true,
            Address = new Address
            {
                Street = "Rua Cliente",
                Number = "456",
                Neighborhood = "Bairro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        context.Properties.Add(property);
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        return (property, customer);
    }

    [Fact]
    public async Task CreateAsync_ValidOffer_ShouldReturnSuccessResponse()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 450000,
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(450000);
        result.Data.OfferStatus.Should().Be(EOfferStatus.Analysis);
    }

    [Fact]
    public async Task CreateAsync_OfferWithHigherValueThanProperty_ShouldStillCreate()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 600000, // Maior que o preço da propriedade
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(600000);
    }

    [Fact]
    public async Task CreateAsync_OfferWithZeroAmount_ShouldCreateSuccessfully()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 0, // Valor zero para teste
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetDefaultSubmissionDate()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 350000,
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.SubmissionDate.Should().NotBeNull();
        result.Data.SubmissionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task UpdateAsync_ValidOffer_ShouldUpdateOfferValue()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 400000,
            OfferStatus = EOfferStatus.Analysis,
            UserId = "user123"
        };
        context.Offers.Add(offer);
        await context.SaveChangesAsync();

        // Atualizar o valor da oferta
        offer.Amount = 420000;

        // Act
        var result = await handler.UpdateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Amount.Should().Be(420000);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPropertyId_ShouldReturnError()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (_, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = 99999, // ID que não existe
            BuyerId = customer.Id,
            Amount = 350000,
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidBuyerId_ShouldReturnError()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, _) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = 99999, // ID que não existe
            Amount = 350000,
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingOffer_ShouldReturnOffer()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 350000,
            UserId = "user123"
        };
        context.Offers.Add(offer);
        await context.SaveChangesAsync();

        var request = new Core.Requests.Offers.GetOfferByIdRequest
        {
            Id = offer.Id,
            UserId = "user123"
        };

        // Act
        var result = await handler.GetByIdAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(350000);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingOffer_ShouldReturnNotFound()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);

        var request = new Core.Requests.Offers.GetOfferByIdRequest
        {
            Id = 99999,
            UserId = "user123"
        };

        // Act
        var result = await handler.GetByIdAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOffersWithPagination()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        // Criar várias ofertas
        for (int i = 1; i <= 15; i++)
        {
            var offer = new Offer
            {
                PropertyId = property.Id,
                BuyerId = customer.Id,
                Amount = 100000 * i,
                UserId = "user123"
            };
            context.Offers.Add(offer);
        }
        await context.SaveChangesAsync();

        var request = new Core.Requests.Offers.GetAllOffersRequest
        {
            UserId = "user123",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(10);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreationDate()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new OfferHandler(context);
        
        var (property, customer) = await SeedTestData(context);
        
        var offer = new Offer
        {
            PropertyId = property.Id,
            BuyerId = customer.Id,
            Amount = 350000,
            UserId = "user123"
        };

        // Act
        var result = await handler.CreateAsync(offer);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}