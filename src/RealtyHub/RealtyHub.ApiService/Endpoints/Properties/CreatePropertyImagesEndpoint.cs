using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class CreatePropertyImagesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:long}/images", HandlerAsync)
            .WithName("Properties: Add Images")
            .WithSummary("Adiciona imagens a um imóvel")
            .WithDescription("Adiciona imagens a um imóvel")
            .WithOrder(6)
            .Produces<Response<PropertyImage?>>();
    }

    private static async Task<IResult> HandlerAsync(
        HttpRequest httpRequest,
        long id,
        IPropertyImageHandler handler,
        ClaimsPrincipal user)
    {
        var request = new CreatePropertyImageRequest
        {
            HttpRequest = httpRequest,
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/property/{id}/images/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }
}