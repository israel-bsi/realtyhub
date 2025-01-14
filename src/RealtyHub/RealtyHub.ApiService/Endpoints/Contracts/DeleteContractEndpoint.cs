﻿using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Contracts;

public class DeleteContractEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:long}", HandlerAsync)
            .WithName("Contracts: Delete")
            .WithSummary("Deleta um contrato")
            .WithDescription("Deleta um contrato")
            .WithOrder(3)
            .Produces<Response<Contract?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IContractHandler handler,
        long id)
    {
        var request = new DeleteContractRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}