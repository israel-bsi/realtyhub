using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Contracts;

public class GetContractByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}", HandlerAsync)
            .WithName("Contracts: Get by Id")
            .WithSummary("Recupera um contrato")
            .WithDescription("Recupera um contrato")
            .WithOrder(4)
            .Produces<Response<Contract?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        long id)
    {
        var request = new GetContractByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetByIdAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}