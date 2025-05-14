using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Customers;

/// <summary>
/// Endpoint responsável por recuperar todos os clientes.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de listagem de clientes.
/// </remarks>
public class GetAllCustomersEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todos os clientes.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna uma lista paginada de clientes associados ao usuário autenticado.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Customers: Get All")
            .WithSummary("Recupera todos os clientes")
            .WithDescription("Recupera todos os clientes")
            .WithOrder(5)
            .Produces<PagedResponse<List<Customer>?>>()
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para recuperar todos os clientes.
    /// </summary>
    /// <remarks>
    /// Este método aplica paginação e permite buscar clientes com base em um termo de pesquisa.
    /// Retorna apenas os clientes associados ao usuário autenticado.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="ICustomerHandler"/> responsável pelas operações relacionadas a clientes.</param>
    /// <param name="pageNumber">Número da página solicitada.</param>
    /// <param name="pageSize">Quantidade de itens por página.</param>
    /// <param name="searchTerm">Termo utilizado para buscar clientes.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a lista paginada de clientes, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize,
        [FromQuery] string searchTerm = "")
    {
        var request = new GetAllCustomersRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };
        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}