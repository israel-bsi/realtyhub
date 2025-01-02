using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Viewings;

public class ScheduleViewingEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Viewings: Schedule")
            .WithSummary("Agenda uma nova visita")
            .WithDescription("Agenda uma nova visita")
            .WithOrder(1)
            .Produces<Response<Viewing?>>();
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        ScheduleViewingRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.ScheduleAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}