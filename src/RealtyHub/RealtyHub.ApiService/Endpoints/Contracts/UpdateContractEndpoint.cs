﻿using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Contracts;

public class UpdateContractEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Contracts: Update")
            .WithSummary("Atualiza um contrato")
            .WithDescription("Atualiza um contrato")
            .WithOrder(2)
            .Produces<Response<Contract?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        UpdateContractRequest request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}