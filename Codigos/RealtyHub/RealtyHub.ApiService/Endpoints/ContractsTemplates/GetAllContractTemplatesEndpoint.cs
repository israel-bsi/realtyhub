using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsTemplates;

/// <summary>
/// Endpoint responsável por recuperar todos os modelos de contrato.
/// </summary>
/// <remarks>
/// <para>Implementa a interface <see cref="IEndpoint"/> para mapear a rota de recuperação dos modelos de contrato.</para>
/// <para>Este endpoint recebe a requisição e chama o handler para buscar os modelos de contrato existentes.</para>
/// </remarks>
public class GetAllContractTemplatesEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint GET para recuperar todos os modelos de contrato.
    /// </summary>
    /// <remarks>
    /// <para>Registra a rota GET que invoca o manipulador <see cref="HandlerAsync"/> para obter os modelos de contrato.</para>
    /// <para>Retorna a lista de modelos de contrato em caso de sucesso ou uma resposta de erro em caso de falha.</para>
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandlerAsync)
            .WithName("ContractTemplates: Get All ContractTemplates")
            .WithSummary("Recupera todos os modelos de contrato")
            .WithDescription("Recupera todos os modelos de contrato")
            .WithOrder(4)
            .Produces<Response<ContractTemplate?>>()
            .Produces<Response<ContractTemplate?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que obtém todos os modelos de contrato.
    /// </summary>
    /// <remarks>
    /// Chama o handler <see cref="IContractTemplateHandler"/> para recuperar os modelos de contrato.
    /// </remarks>
    /// <param name="handler">Handler responsável pelas operações dos modelos de contrato.</param>
    /// <returns>
    /// Retorna um objeto do tipo <see cref="IResult"/> representando a resposta HTTP,
    /// com status 200 OK em caso de sucesso ou 400 BadRequest em caso de falha.
    /// </returns>
    public static async Task<IResult> HandlerAsync(
        IContractTemplateHandler handler)
    {
        var result = await handler.GetAllAsync();

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}