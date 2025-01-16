using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class CreatePropertyPhotosEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Add Photos")
            .WithSummary("Adiciona fotos a um imóvel")
            .WithDescription("Adiciona fotos a um imóvel")
            .WithOrder(6)
            .Produces<Response<PropertyPhoto?>>();
    }

    private static async Task<IResult> HandlerAsync(
        HttpRequest httpRequest,
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user)
    {
        var request = new CreatePropertyPhotosRequest
        {
            HttpRequest = httpRequest,
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.CreatedAtRoute(value: result)
            : Results.BadRequest(result);
    }
}