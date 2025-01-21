using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

public class UpdateOfferEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Offers: Update")
            .WithSummary("Atualiza uma proposta")
            .WithDescription("Atualiza uma proposta")
            .WithOrder(2)
            .Produces<Response<Offer?>>()
            .Produces<Response<Offer?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        UpdateOfferRequest request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}