using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;

namespace RealtyHub.ApiService.Endpoints.Offers;

public class GetAllOffersByCustomerEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/customer/{id:long}", HandlerAsync)
            .WithName("Offers: Get All by Customer")
            .WithSummary("Recupera todas as propostas feitas por um cliente")
            .WithDescription("Recupera todas as propostas feitas por um cliente")
            .WithOrder(8)
            .Produces<Response<Offer?>>();
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id)
    {
        var request = new GetAllOffersByCustomerRequest
        {
            CustomerId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetAllOffersByCustomerAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}