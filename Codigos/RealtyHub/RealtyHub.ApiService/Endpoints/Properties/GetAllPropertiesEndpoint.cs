using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllPropertiesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Properties: Get All")
            .WithSummary("Recupera todos os imóvels")
            .WithDescription("Recupera todos os imóvels")
            .WithOrder(5)
            .Produces<PagedResponse<List<Property>?>>()
            .Produces<PagedResponse<Property?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        [FromQuery] string filterBy = "",
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize,
        [FromQuery] string searchTerm = "")
    {
        var request = new GetAllPropertiesRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            FilterBy = filterBy
        };

        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}