using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Net.Http.Json;

namespace RealtyHub.Tests.Examples;

/// <summary>
/// EXEMPLO: Como usar as diferentes abordagens de teste
/// </summary>
/// <remarks>
/// Esta classe demonstra as duas abordagens disponíveis:
/// 
/// 1. RealtyHubApiTests: Para testes de VALIDAÇÃO e LÓGICA DE NEGÓCIO (autorização DESABILITADA)
///    - Foco em validação de campos
///    - Testes de CRUD
///    - Regras de negócio
///    - Performance dos endpoints
/// 
/// 2. RealtyHubApiTestsWithAuth: Para testes de AUTENTICAÇÃO (autorização HABILITADA)
///    - Fluxos de login/logout
///    - Validação de tokens
///    - Políticas de autorização
///    - Segurança
/// </remarks>
public class ExampleTestUsage : IClassFixture<RealtyHubApiTests>
{
    private readonly RealtyHubApiTests _factory;

    public ExampleTestUsage(RealtyHubApiTests factory)
    {
        _factory = factory;
    }

    #region Exemplo: Testes de Validação (SEM Autenticação)

    [Fact]
    public async Task EXEMPLO_ValidacaoCampos_SemAutenticacao()
    {
        // ? VANTAGENS desta abordagem:
        // - Foco total na validação
        // - Testes mais rápidos
        // - Não precisa lidar com confirmação de email
        // - Banco sempre limpo
        // - Fácil de configurar dados de teste

        // Arrange
        var client = MockData.CreateSimpleClient(_factory);
        var customerInvalido = new Customer
        {
            // Testando validações específicas
            Name = "", // ? Campo obrigatório vazio
            Email = "email-invalido", // ? Email em formato inválido
            Phone = "telefone-invalido", // ? Telefone inválido
            DocumentNumber = new string('1', 25) // ? Documento muito longo
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/customers", customerInvalido);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        // ? Aqui você pode focar em:
        // - Quais campos estão sendo validados
        // - Se as mensagens de erro estão corretas
        // - Se as regras de negócio estão funcionando
        // - Performance do endpoint
    }

    [Fact]
    public async Task EXEMPLO_TesteCompleto_CRUD_SemAutenticacao()
    {
        // Arrange
        var client = MockData.CreateSimpleClient(_factory);
        
        // CREATE
        var customer = new Customer
        {
            Name = "João Teste",
            Email = "joao@test.com",
            Phone = "11999999999",
            DocumentNumber = "12345678901",
            CustomerType = Core.Enums.ECustomerType.Buyer,
            PersonType = Core.Enums.EPersonType.Individual,
            Address = new Address
            {
                Street = "Rua Teste",
                Number = "123",
                City = "São Paulo",
                State = "SP",
                Country = "Brasil",
                ZipCode = "01234-567"
            }
        };

        var createResponse = await client.PostAsJsonAsync("/v1/customers", customer);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert CREATE
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCustomer!.Data!.Id.Should().BeGreaterThan(0);

        var customerId = createdCustomer.Data.Id;

        // READ
        var getResponse = await client.GetAsync($"/v1/customers/{customerId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert READ
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResult!.Data!.Name.Should().Be("João Teste");

        // UPDATE
        getResult.Data.Name = "João Atualizado";
        var updateResponse = await client.PutAsJsonAsync($"/v1/customers/{customerId}", getResult.Data);
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<Response<Customer>>();

        // Assert UPDATE
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateResult!.Data!.Name.Should().Be("João Atualizado");

        // DELETE
        var deleteResponse = await client.DeleteAsync($"/v1/customers/{customerId}");

        // Assert DELETE
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion (soft delete)
        var getDeletedResponse = await client.GetAsync($"/v1/customers/{customerId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Exemplo: Quando usar cada abordagem

    /// <summary>
    /// ? USE RealtyHubApiTests (sem auth) PARA:
    /// - Validação de campos obrigatórios
    /// - Validação de formatos (email, telefone, etc.)
    /// - Validação de tamanho máximo/mínimo
    /// - Testes de regras de negócio
    /// - Testes de performance
    /// - Testes de CRUD completo
    /// - Testes de paginação e filtros
    /// - Testes de relacionamentos entre entidades
    /// </summary>
    [Fact]
    public void QuandoUsar_SemAutenticacao()
    {
        // Exemplo de cenários ideais:
        var cenarios = new[]
        {
            "Validar se nome é obrigatório",
            "Validar formato de email",
            "Testar limite de caracteres",
            "Verificar se dados são salvos corretamente",
            "Testar paginação",
            "Verificar filtros de busca",
            "Testar soft delete",
            "Validar relacionamentos",
            "Performance de queries"
        };

        cenarios.Should().HaveCount(9);
    }

    /// <summary>
    /// ? USE RealtyHubApiTestsWithAuth (com auth) PARA:
    /// - Fluxos de registro e login
    /// - Validação de tokens
    /// - Testes de autorização (quem pode acessar o quê)
    /// - Políticas de segurança
    /// - Confirmação de email
    /// - Reset de senha
    /// - Expiração de sessão
    /// </summary>
    [Fact]
    public void QuandoUsar_ComAutenticacao()
    {
        // Exemplo de cenários ideais:
        var cenarios = new[]
        {
            "Usuário não autenticado não pode acessar endpoint protegido",
            "Login com credenciais válidas funciona",
            "Token expirado é rejeitado",
            "Usuário só vê seus próprios dados",
            "Diferentes roles têm acessos diferentes",
            "Logout invalida o token",
            "Confirmação de email é obrigatória"
        };

        cenarios.Should().HaveCount(7);
    }

    #endregion
}