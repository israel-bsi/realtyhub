using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Offers;

public class CreateOfferEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Offers: Create")
            .WithSummary("Cria uma proposta")
            .WithDescription("Cria uma proposta")
            .WithOrder(1)
            .Produces<Response<Offer?>>(StatusCodes.Status201Created)
            .Produces<Response<Offer?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        CreateOfferRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}