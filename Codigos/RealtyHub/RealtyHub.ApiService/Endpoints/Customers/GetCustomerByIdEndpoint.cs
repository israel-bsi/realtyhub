using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Customers;

public class GetCustomerByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Customers: Get by Id")
            .WithSummary("Recupera um cliente")
            .WithDescription("Recupera um cliente")
            .WithOrder(4)
            .Produces<Response<Customer?>>()
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        long id)
    {
        var request = new GetCustomerByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}