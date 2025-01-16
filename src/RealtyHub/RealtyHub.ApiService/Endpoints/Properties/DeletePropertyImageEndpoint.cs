using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;
public class DeletePropertyImageEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{propertyId:long}/images/{imageId}", HandlerAsync)
            .WithName("Properties: Delete Photos")
            .WithSummary("Exclui uma foto de um imóvel")
            .WithDescription("Exclui uma foto de um imóvel")
            .WithOrder(7)
            .Produces<Response<PropertyImage?>>();
    }

    private static async Task<IResult> HandlerAsync(
        long propertyId,
        IPropertyImageHandler handler,
        ClaimsPrincipal user, 
        string imageId)
    {
        var request = new DeletePropertyImageRequest
        {
            ImageId = imageId,
            PropertyId = propertyId,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.DeleteAsync(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }
}