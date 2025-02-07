using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

public class GetAllCondominiumsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/", HandlerAsync)
            .WithName("Condominiums: Get All")
            .WithSummary("Obtém todos os condomínios")
            .WithDescription("Obtém todos os condomínios")
            .WithOrder(5)
            .Produces<PagedResponse<List<Condominium>>>()
            .Produces<PagedResponse<List<Condominium>>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        [FromQuery] string filterBy = "",
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize,
        [FromQuery] string searchterm = "")
    {
        var request = new GetAllCondominiumsRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchterm,
            FilterBy = filterBy
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}