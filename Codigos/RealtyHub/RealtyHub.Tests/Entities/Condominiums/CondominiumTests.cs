using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Handlers;
using RealtyHub.Core.Models;

namespace RealtyHub.Tests.Entities.Condominiums;

public class CondominiumTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ValidCondominium_ShouldReturnSuccessResponse()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var condominium = new Condominium
        {
            Name = "Residencial Teste",
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
            Floors = 10,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = false,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 350.50m
        };

        // Act
        var result = await handler.CreateAsync(condominium);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Residencial Teste");
        result.Data.IsActive.Should().BeTrue();
        result.Data.HasElevator.Should().BeTrue();
        result.Data.CondominiumValue.Should().Be(350.50m);
    }

    [Fact]
    public async Task CreateAsync_CondominiumWithoutOptionalFeatures_ShouldCreateSuccessfully()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var condominium = new Condominium
        {
            Name = "Edifício Simples",
            Address = new Address
            {
                Street = "Rua Secundária",
                Number = "200",
                Neighborhood = "Bairro Novo",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 20,
            Floors = 4,
            HasElevator = false,
            HasSwimmingPool = false,
            HasPartyRoom = false,
            HasPlayground = false,
            HasFitnessRoom = false,
            CondominiumValue = 150.00m
        };

        // Act
        var result = await handler.CreateAsync(condominium);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.HasElevator.Should().BeFalse();
        result.Data.HasSwimmingPool.Should().BeFalse();
        result.Data.CondominiumValue.Should().Be(150.00m);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCondominium_ShouldReturnCondominium()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var condominium = new Condominium
        {
            Name = "Condomínio Existente",
            Address = new Address
            {
                Street = "Endereço Teste",
                Number = "123",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 30,
            IsActive = true
        };
        context.Condominiums.Add(condominium);
        await context.SaveChangesAsync();

        // Act
        var result = await handler.GetByIdAsync(new Core.Requests.Condominiums.GetCondominiumByIdRequest 
        { 
            Id = condominium.Id,
            UserId = "user123"
        });

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Condomínio Existente");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCondominium_ShouldReturnNotFound()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);

        // Act
        var result = await handler.GetByIdAsync(new Core.Requests.Condominiums.GetCondominiumByIdRequest 
        { 
            Id = 999,
            UserId = "user123"
        });

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingCondominium_ShouldSetInactive()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var condominium = new Condominium
        {
            Name = "Condomínio para Deletar",
            Address = new Address
            {
                Street = "Endereço Delete",
                Number = "999",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 25,
            IsActive = true
        };
        context.Condominiums.Add(condominium);
        await context.SaveChangesAsync();

        var deleteRequest = new Core.Requests.Condominiums.DeleteCondominiumRequest
        {
            Id = condominium.Id,
            UserId = "user123"
        };

        // Act
        var result = await handler.DeleteAsync(deleteRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var deletedCondominium = await context.Condominiums.FindAsync(condominium.Id);
        deletedCondominium!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_WithActiveCondominiums_ShouldReturnOnlyActive()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var address1 = new Address { Street = "End 1", Number = "1", Neighborhood = "Centro", City = "SP", State = "SP", Country = "Brasil", ZipCode = "01234-567" };
        var address2 = new Address { Street = "End 2", Number = "2", Neighborhood = "Centro", City = "SP", State = "SP", Country = "Brasil", ZipCode = "01234-567" };
        var address3 = new Address { Street = "End 3", Number = "3", Neighborhood = "Centro", City = "SP", State = "SP", Country = "Brasil", ZipCode = "01234-567" };
        
        // Criar condomínios ativos e inativos
        context.Condominiums.AddRange(
            new Condominium { Name = "Ativo 1", Address = address1, Units = 10, IsActive = true },
            new Condominium { Name = "Ativo 2", Address = address2, Units = 20, IsActive = true },
            new Condominium { Name = "Inativo 1", Address = address3, Units = 15, IsActive = false }
        );
        await context.SaveChangesAsync();

        var request = new Core.Requests.Condominiums.GetAllCondominiumsRequest
        {
            UserId = "user123",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2); // Apenas os ativos
        result.Data.Should().OnlyContain(c => c.IsActive);
    }

    [Fact]
    public async Task CreateAsync_CondominiumWithHighValue_ShouldHandleDecimalCorrectly()
    {
        // Arrange
        await using var context = GetInMemoryDbContext();
        var handler = new CondominiumHandler(context);
        
        var condominium = new Condominium
        {
            Name = "Condomínio Luxo",
            Address = new Address
            {
                Street = "Av. Luxo",
                Number = "5000",
                Neighborhood = "Alto Padrão",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            },
            Units = 100,
            Floors = 25,
            HasElevator = true,
            HasSwimmingPool = true,
            HasPartyRoom = true,
            HasPlayground = true,
            HasFitnessRoom = true,
            CondominiumValue = 1250.75m // Valor alto com decimais
        };

        // Act
        var result = await handler.CreateAsync(condominium);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CondominiumValue.Should().Be(1250.75m);
    }
}