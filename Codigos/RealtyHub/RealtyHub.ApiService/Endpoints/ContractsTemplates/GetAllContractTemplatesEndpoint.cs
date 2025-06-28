using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsTemplates;

/// <summary>
/// Endpoint responsável por recuperar todos os modelos de contrato.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de listagem de modelos de contrato.
/// </remarks>
public class GetAllContractTemplatesEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para recuperar todos os modelos de contrato.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna uma lista de todos os modelos de contrato disponíveis.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandlerAsync)
            .WithName("ContractTemplates: Get All ContractTemplates")
            .WithSummary("Recupera todos os modelos de contrato")
            .WithDescription("Recupera todos os modelos de contrato")
            .WithOrder(4)
            .Produces<Response<ModeloContrato?>>()
            .Produces<Response<ModeloContrato?>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que recebe a requisição para recuperar todos os modelos de contrato.
    /// </summary>
    /// <remarks>
    /// Este método chama o handler para buscar todos os modelos de contrato disponíveis e retorna o resultado.
    /// </remarks>
    /// <param name="handler">Instância de <see cref="IContractTemplateHandler"/> responsável pelas operações relacionadas a modelos de contrato.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a lista de modelos de contrato, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição.</para>
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