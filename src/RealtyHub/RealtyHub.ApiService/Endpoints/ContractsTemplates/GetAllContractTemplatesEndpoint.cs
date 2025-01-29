using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsTemplates;

public class GetAllContractTemplatesEndpoint : IEndpoint
{
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

    private static async Task<IResult> HandlerAsync(
        IContractTemplateHandler handler,
        ClaimsPrincipal user)
    {
        var result = await handler.GetAllAsync();

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}