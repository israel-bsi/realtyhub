﻿using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

public class RescheduleViewingEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}/reschedule", HandlerAsync)
            .WithName("Viewings: Reschedule")
            .WithSummary("Reagenda uma visita")
            .WithDescription("Reagenda uma visita")
            .WithOrder(2)
            .Produces<Response<Viewing?>>()
            .Produces<Response<Viewing?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        Viewing request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.RescheduleAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}