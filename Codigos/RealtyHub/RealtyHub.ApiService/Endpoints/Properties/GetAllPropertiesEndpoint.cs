using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por recuperar todos os imóveis.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de listagem de todos os imóveis.
/// </remarks>
public class GetAllPropertiesEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todos os imóveis.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna uma lista paginada de todos os imóveis, com suporte a filtros e pesquisa.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Properties: Get All")
            .WithSummary("Recupera todos os imóveis")
            .WithDescription("Recupera todos os imóveis")
            .WithOrder(5)
            .Produces<PagedResponse<List<Property>?>>()
            .Produces<PagedResponse<Property?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que retorna todos os imóveis.
    /// </summary>
    /// <remarks>
    /// Este método cria uma requisição para buscar todos os imóveis, aplicando filtro, paginação e termos de pesquisa,
    /// e chama o handler para processar a requisição.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IPropertyHandler"/> responsável pelas operações relacionadas a imóveis.</param>
    /// <param name="filterBy">Filtro para os imóveis <see cref="string"/> (opcional).</param>
    /// <param name="pageNumber">Número da página solicitada <see cref="int"/> (padrão: <see cref="Core.Configuration.DefaultPageNumber"/>).</param>
    /// <param name="pageSize">Quantidade de itens por página <see cref="int"/> (padrão: <see cref="Core.Configuration.DefaultPageSize"/>).</param>
    /// <param name="searchTerm">Termo de pesquisa para buscar imóveis <see cref="string"/> (opcional).</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a lista paginada de imóveis, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        [FromQuery] string filterBy = "",
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize,
        [FromQuery] string searchTerm = "")
    {
        var request = new GetAllPropertiesRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            FilterBy = filterBy
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}