using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class UpdatePropertyPhotosEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Update Photos")
            .WithSummary("Atualiza as fotos de um imóvel")
            .WithDescription("Atualiza as fotos de um imóvel")
            .WithOrder(7)
            .Produces<Response<List<PropertyPhoto>?>>()
            .Produces<Response<List<PropertyPhoto>?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        UpdatePropertyPhotosRequest request,
        ClaimsPrincipal user)
    {
        request.PropertyId = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}