using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsContent;

public class CreateContractContentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandlerAsync)
            .WithName("ContractContent: Create ContractContent")
            .WithSummary("Cria um modelo de contrato")
            .WithDescription("Cria um modelo de contrato")
            .WithOrder(4)
            .Produces<Response<ContractContent?>>()
            .Produces<Response<ContractContent?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        CreateContractContentRequest request,
        IContractContentHandler handler)
    {
        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}