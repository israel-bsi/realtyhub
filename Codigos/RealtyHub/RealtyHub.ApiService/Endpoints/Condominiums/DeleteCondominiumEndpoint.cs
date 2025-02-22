using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

public class DeleteCondominiumEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Condominiums: Delete")
            .WithSummary("Deleta um condomínio")
            .WithDescription("Deleta um condomínio")
            .WithOrder(4)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<Response<Condominium?>>(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        long id)
    {
        var request = new DeleteCondominiumRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result);
    }
}