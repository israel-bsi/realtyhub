using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Customers;

public class GetAllCustomersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Customers: Get All")
            .WithSummary("Recupera todos os clientes")
            .WithDescription("Recupera todos os clientes")
            .WithOrder(5)
            .Produces<PagedResponse<List<Customer>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllCustomersRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await handler.GetAllAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}