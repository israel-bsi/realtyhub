using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllViewingsByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:long}/viewings", HandlerAsync)
            .WithName("Properties: Get All Viewings")
            .WithSummary("Lista todas as visitas de um imóvel")
            .WithDescription("Lista todas as visitas de um imóvel")
            .WithOrder(6)
            .Produces<Response<List<Viewing>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPropertyHandler handler,
        long id)
    {
        var request = new GetAllViewingsByPropertyRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PropertyId = id
        };

        var result = await handler.GetAllViewingsAsync(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}