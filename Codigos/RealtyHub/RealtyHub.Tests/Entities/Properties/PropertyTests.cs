using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests.Entities.Properties;

public class PropertyTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ValidProperty_ShouldReturnSuccessResponse()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        var property = new Property
        {
            Title = "Casa de Teste",
            Description = "Descrição de teste",
            Price = 500000,
            Address = new Address
            {
                Street = "Rua de Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            PropertyType = Core.Enums.EPropertyType.House,
            Bedroom = 3,
            Bathroom = 2,
            Area = 150,
            Garage = 2,
            IsNew = true,
            RegistryNumber = "12345",
            RegistryRecord = "Livro 1",
            TransactionsDetails = "Detalhes da transação",
            UserId = "user123",
            ShowInHome = true
        };

        // Act
        var result = await handler.CreateAsync(property);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Casa de Teste");
        result.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_PropertyWithCondominium_ShouldLinkCorrectly()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        // Criar um condomínio primeiro
        var condominium = new Condominium
        {
            Name = "Condomínio Teste",
            Address = new Address
            {
                Street = "Av. Principal",
                Number = "1000",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 50,
            IsActive = true
        };
        context.Condominiums.Add(condominium);
        await context.SaveChangesAsync();
        
        var property = new Property
        {
            Title = "Apartamento de Teste",
            Price = 300000,
            PropertyType = Core.Enums.EPropertyType.Apartment,
            UserId = "user123",
            CondominiumId = condominium.Id,
            Address = new Address
            {
                Street = "Av. Principal",
                Number = "1000",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        // Act
        var result = await handler.CreateAsync(property);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CondominiumId.Should().Be(condominium.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProperty_ShouldReturnProperty()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        var property = new Property
        {
            Title = "Casa Existente",
            Price = 400000,
            UserId = "user123",
            IsActive = true,
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "100",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        // Act
        var result = await handler.GetByIdAsync(new Core.Requests.Properties.GetPropertyByIdRequest 
        { 
            Id = property.Id, 
            UserId = "user123" 
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Casa Existente");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProperty_ShouldReturnNotFound()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);

        // Act
        var result = await handler.GetByIdAsync(new Core.Requests.Properties.GetPropertyByIdRequest 
        { 
            Id = 999, 
            UserId = "user123" 
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingProperty_ShouldSetInactive()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        var property = new Property
        {
            Title = "Casa para Deletar",
            UserId = "user123",
            IsActive = true,
            Address = new Address
            {
                Street = "Rua Delete",
                Number = "999",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var deleteRequest = new Core.Requests.Properties.DeletePropertyRequest
        {
            Id = property.Id,
            UserId = "user123"
        };

        // Act
        var result = await handler.DeleteAsync(deleteRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var deletedProperty = await context.Properties.FindAsync(property.Id);
        deletedProperty!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        // Criar várias propriedades
        for (int i = 1; i <= 15; i++)
        {
            context.Properties.Add(new Property
            {
                Title = $"Casa {i}",
                Price = 100000 * i,
                UserId = "user123",
                IsActive = true,
                Address = new Address
                {
                    Street = $"Rua {i}",
                    Number = i.ToString(),
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    Country = "Brasil",
                    ZipCode = "01234-567"
                }
            });
        }
        await context.SaveChangesAsync();

        var request = new Core.Requests.Properties.GetAllPropertiesRequest
        {
            UserId = "user123",
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = await handler.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(5); // Restam 5 itens na página 2
    }

    [Fact]
    public async Task CreateAsync_PropertyWithNegativePrice_ShouldStillCreate()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        var property = new Property
        {
            Title = "Casa com Preço Negativo",
            Price = -100000, // Preço negativo para teste
            UserId = "user123",
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "1",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        // Act
        var result = await handler.CreateAsync(property);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(-100000);
    }

    [Fact]
    public async Task CreateAsync_PropertyWithZeroPrice_ShouldCreate()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new PropertyHandler(context);
        
        var property = new Property
        {
            Title = "Casa Gratuita",
            Price = 0, // Preço zero
            UserId = "user123",
            Address = new Address
            {
                Street = "Rua Gratuita",
                Number = "0",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        // Act
        var result = await handler.CreateAsync(property);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Price.Should().Be(0);
    }
}