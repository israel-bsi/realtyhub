using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

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
            .Produces<PagedResponse<Offer?>>()
            .Produces<PagedResponse<Offer?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IOfferHandler handler,
        long id,
        [FromQuery] string? startDate,
        [FromQuery] string? endDate,
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize)
    {
        var request = new GetAllOffersByCustomerRequest
        {
            CustomerId = id,
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await handler.GetAllOffersByCustomerAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}