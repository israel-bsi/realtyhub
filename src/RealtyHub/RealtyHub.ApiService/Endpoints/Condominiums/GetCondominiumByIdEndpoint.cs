using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

public class GetCondominiumByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Condominiums: Get by id")
            .WithSummary("Recupera um condomínio")
            .WithDescription("Recupera um condomínio")
            .WithOrder(3)
            .Produces<Response<Condominium?>>()
            .Produces<Response<Condominium?>>(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        long id)
    {
        var request = new GetCondominiumByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound(result);
    }
}