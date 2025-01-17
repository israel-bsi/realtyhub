using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;
using RealtyHub.Core.Requests.PropertiesPhotos;

namespace RealtyHub.ApiService.Endpoints.Properties;
public class DeletePropertyPhotoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:long}/photos/{photoId}", HandlerAsync)
            .WithName("Properties: Delete Photos")
            .WithSummary("Exclui uma foto de um imóvel")
            .WithDescription("Exclui uma foto de um imóvel")
            .WithOrder(7)
            .Produces<Response<PropertyPhoto?>>();
    }

    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user, 
        string photoId)
    {
        var request = new DeletePropertyPhotoRequest
        {
            Id = photoId,
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}