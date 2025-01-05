using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Contracts;

public class GetAllContractsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Contracts: Get All")
            .WithSummary("Recupera todos os contratos")
            .WithDescription("Recupera todos os contratos")
            .WithOrder(5)
            .Produces<PagedResponse<List<Contract>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllContractsRequest
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