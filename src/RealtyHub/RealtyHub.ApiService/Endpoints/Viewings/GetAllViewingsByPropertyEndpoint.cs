using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Viewings;

public class GetAllViewingsByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/properties/{id:long}", HandlerAsync)
            .WithName("Viewings: Get All By Property")
            .WithSummary("Lista todas as visitas de um imóvel")
            .WithDescription("Lista todas as visitas de um imóvel")
            .WithOrder(6)
            .Produces<Response<List<Viewing>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IViewingHandler handler,
        long id)
    {
        var request = new GetAllViewingsByPropertyRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PropertyId = id
        };
        
        var result = await handler.GetAllByProperty(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}