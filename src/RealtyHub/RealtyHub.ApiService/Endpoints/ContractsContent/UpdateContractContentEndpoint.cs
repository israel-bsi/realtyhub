using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsContent;

public class UpdateContractContentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id}", HandlerAsync)
            .WithName("ContractContent: Update ContractContent")
            .WithSummary("Atualiza um modelo de contrato")
            .WithDescription("Atualiza um modelo de contrato")
            .WithOrder(3)
            .Produces<Response<ContractContent?>>()
            .Produces<Response<ContractContent?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        string id,
        IContractContentHandler handler,
        ClaimsPrincipal user)
    {
        var request = new UpdateContractContentRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}