using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Offers;

public class GetAllOffersByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/property/{id:long}", HandlerAsync)
            .WithName("Offers: Get All by Property")
            .WithSummary("Recupera todas as propostas de um imóvel")
            .WithDescription("Recupera todas as propostas de um imóvel")
            .WithOrder(6)
            .Produces<Response<Offer?>>();
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new GetAllOffersByPropertyRequest
        {
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetAllOffersByPropertyAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}