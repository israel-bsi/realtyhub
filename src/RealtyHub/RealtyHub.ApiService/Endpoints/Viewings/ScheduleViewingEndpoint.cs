using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Viewings;

public class ScheduleViewingEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Viewings: Schedule")
            .WithSummary("Agenda uma nova visita")
            .WithDescription("Agenda uma nova visita")
            .WithOrder(1)
            .Produces<Response<Viewing?>>(StatusCodes.Status201Created)
            .Produces<Response<Viewing?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        Viewing request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.ScheduleAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}