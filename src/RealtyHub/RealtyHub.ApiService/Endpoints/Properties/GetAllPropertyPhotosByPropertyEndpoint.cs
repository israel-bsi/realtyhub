using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;
using RealtyHub.Core.Requests.PropertiesPhotos;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllPropertyPhotosByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{propertyId:long}/photos", HandlerAsync)
            .WithName("Properties: Get Photos By Property")
            .WithSummary("Recupera todos as fotos de um imóvel")
            .WithDescription("Recupera todos as fotos de um imóvel")
            .WithOrder(8)
            .Produces<Response<List<PropertyPhoto>?>>();
    }

    private static async Task<IResult> HandlerAsync(
        long propertyId,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetAllPropertyPhotosByPropertyRequest
        {
            PropertyId = propertyId,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetAllByPropertyAsync(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}