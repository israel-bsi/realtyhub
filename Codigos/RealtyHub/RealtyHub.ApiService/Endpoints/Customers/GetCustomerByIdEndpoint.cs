using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Customers;

/// <summary>
/// Endpoint responsável por recuperar um cliente específico pelo seu ID.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção de um cliente pelo ID.
/// </remarks>
public class GetCustomerByIdEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar um cliente pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que espera um parâmetro numérico (ID) e chama o manipulador para retornar o cliente correspondente.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Customers: Get by Id")
            .WithSummary("Recupera um cliente")
            .WithDescription("Recupera um cliente")
            .WithOrder(4)
            .Produces<Response<Customer?>>()
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para recuperar um cliente pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Este método constrói uma requisição com o ID extraído da rota e o usuário autenticado,
    /// chama o handler para obter o cliente e retorna uma resposta adequada com base no resultado.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="ICustomerHandler"/> responsável pelas operações relacionadas a clientes.</param>
    /// <param name="id">ID do cliente a ser recuperado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes do cliente, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        long id)
    {
        var request = new GetCustomerByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}