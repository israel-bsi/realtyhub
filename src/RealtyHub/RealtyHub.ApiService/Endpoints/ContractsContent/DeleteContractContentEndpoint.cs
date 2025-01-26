using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.ContractsContent;

public class DeleteContractContentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", HandlerAsync)
            .WithName("ContractContent: Delete ContractContent")
            .WithSummary("Deleta um modelo de contrato")
            .WithDescription("Deleta um modelo de contrato")
            .WithOrder(5)
            .Produces<Response<ContractContent?>>()
            .Produces<Response<ContractContent?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        string id,
        IContractContentHandler handler,
        ClaimsPrincipal user)
    {
        var request = new DeleteContractContentRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}