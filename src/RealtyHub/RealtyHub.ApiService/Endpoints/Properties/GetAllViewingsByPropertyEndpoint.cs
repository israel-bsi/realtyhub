using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllViewingsByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}/viewings", HandlerAsync)
            .WithName("Properties: Get All Viewings")
            .WithSummary("Lista todas as visitas de um imóvel")
            .WithDescription("Lista todas as visitas de um imóvel")
            .WithOrder(6)
            .Produces<PagedResponse<List<Viewing>?>>()
            .Produces<PagedResponse<Property?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        long id,
        [FromQuery] string? startDate, 
        [FromQuery] string? endDate,
        [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Core.Configuration.DefaultPageSize)
    {
        var request = new GetAllViewingsByPropertyRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PropertyId = id,
            PageNumber = pageNumber,
            PageSize = pageSize,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await handler.GetAllViewingsAsync(request);

        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.BadRequest(result);
    }
}