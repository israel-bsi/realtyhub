using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

public class GetOfferAcceptedEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/property/{id:long}/accepted", HandlerAsync)
            .WithName("Offers: Get Accepted")
            .WithSummary("Recupera a proposta aceita")
            .WithDescription("Recupera a proposta aceita")
            .WithOrder(4)
            .Produces<Response<Offer?>>()
            .Produces<Response<Offer?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new GetOfferAcceptedByProperty
        {
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetAcceptedByProperty(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}