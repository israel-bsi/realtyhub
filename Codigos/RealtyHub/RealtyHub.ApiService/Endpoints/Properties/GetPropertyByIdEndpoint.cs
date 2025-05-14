using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por recuperar um imóvel específico pelo seu ID.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção de um imóvel pelo ID.
/// </remarks>
public class GetPropertyByIdEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar um imóvel pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna os detalhes de um imóvel específico com base no ID fornecido.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Properties: Get by Id")
            .WithSummary("Recupera um imóvel")
            .WithDescription("Recupera um imóvel")
            .WithOrder(4)
            .Produces<Response<Property?>>()
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna os detalhes de um imóvel pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar um imóvel específico com base no ID fornecido
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IPropertyHandler"/> responsável pelas operações relacionadas a imóveis.</param>
    /// <param name="id">ID do imóvel a ser recuperado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes do imóvel, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        long id)
    {
        var request = new GetPropertyByIdRequest
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