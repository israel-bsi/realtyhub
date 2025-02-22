using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts;

public class DeleteContractEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Contracts: Delete")
            .WithSummary("Deleta um contrato")
            .WithDescription("Deleta um contrato")
            .WithOrder(3)
            .Produces<Response<Contract?>>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        long id)
    {
        var request = new DeleteContractRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(result);
    }
}