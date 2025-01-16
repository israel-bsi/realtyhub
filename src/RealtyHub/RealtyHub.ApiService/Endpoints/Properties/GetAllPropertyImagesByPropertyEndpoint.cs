using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllPropertyImagesByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{propertyId:long}/images", HandlerAsync)
            .WithName("Properties: Get Photos By Property")
            .WithSummary("Recupera todos as fotos de um imóvel")
            .WithDescription("Recupera todos as fotos de um imóvel")
            .WithOrder(8)
            .Produces<Response<List<PropertyImage>?>>();
    }

    private static async Task<IResult> HandlerAsync(
        long propertyId,
        IPropertyImageHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetAllPropertyImagesByPropertyRequest
        {
            PropertyId = propertyId,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetAllByPropertyAsync(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}