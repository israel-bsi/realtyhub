using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

/// <summary>
/// Endpoint responsável por atualizar um imóvel existente.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de imóveis.
/// </remarks>
public class UpdatePropertyEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar um imóvel.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que recebe os dados atualizados do imóvel e o ID do imóvel a ser modificado.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Properties: Update")
            .WithSummary("Atualiza um imóvel")
            .WithDescription("Atualiza um imóvel")
            .WithOrder(2)
            .Produces<Response<Imovel?>>()
            .Produces<Response<Imovel?>>(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que atualiza um imóvel existente.
    /// </summary>
    /// <remarks>
    /// Este método recebe os dados atualizados do imóvel, associa o ID do usuário autenticado ao imóvel,
    /// e chama o handler para realizar a atualização.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="IPropertyHandler"/> responsável pelas operações relacionadas a imóveis.</param>
    /// <param name="request">Objeto <see cref="Imovel"/> contendo os dados atualizados do imóvel.</param>
    /// <param name="id">ID do imóvel a ser atualizado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os detalhes do imóvel atualizado, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        Imovel request,
        long id)
    {
        request.Id = id;
        request.UsuarioId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}