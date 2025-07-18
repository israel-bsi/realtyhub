using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Common.Validation;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Customers;

/// <summary>
/// Endpoint responsável por criar novos clientes.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de criação de clientes.
/// Aplica validação de Data Annotations conforme definido no modelo Customer.
/// </remarks>
public class CreateCustomerEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para criar um cliente.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados do cliente e chama o manipulador para criar o cliente.
    /// Aplica validação automática de Data Annotations antes do processamento.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Customers: Create")
            .WithSummary("Cria um novo cliente")
            .WithDescription("Cria um novo cliente com validação de Data Annotations")
            .WithOrder(1)
            .Produces<Response<Customer?>>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para criar um cliente.
    /// </summary>
    /// <remarks>
    /// Este método:
    /// 1. Valida os dados do cliente usando APENAS os Data Annotations definidos no modelo
    /// 2. Associa o ID do usuário autenticado
    /// 3. Chama o handler para criar o novo cliente
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="ICustomerHandler"/> responsável pelas operações relacionadas a clientes.</param>
    /// <param name="request">Objeto <see cref="Customer"/> contendo os dados para criação do cliente.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 201 Created com os dados do cliente criado, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request com erros de validação dos Data Annotations.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        Customer request)
    {
        // Validação APENAS dos Data Annotations
        var validationResult = DataAnnotationValidator.ValidateRecursively(request);
        if (validationResult != null)
        {
            return validationResult; // Retorna BadRequest com erros dos Data Annotations
        }

        // Processamento normal
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}