using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsContent;

public class GetAllContractContentByUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandlerAsync)
            .WithName("ContractContent: Get ContractsContent By User")
            .WithSummary("Recupera todos os modelos de contrato")
            .WithDescription("Recupera todos os modelos de contrato")
            .WithOrder(4)
            .Produces<Response<List<PropertyPhoto>?>>()
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        IContractContentHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetAllContractContentByUserRequest
        {
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetAllByUserAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}