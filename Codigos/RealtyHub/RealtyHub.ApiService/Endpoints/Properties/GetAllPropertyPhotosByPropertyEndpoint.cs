﻿using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Properties;

public class GetAllPropertyPhotosByPropertyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:long}/photos", HandlerAsync)
            .WithName("Properties: Get Photos By Property")
            .WithSummary("Recupera todos as fotos de um imóvel")
            .WithDescription("Recupera todos as fotos de um imóvel")
            .WithOrder(8)
            .Produces<Response<List<PropertyPhoto>?>>()
            .Produces<Response<Property?>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        long id,
        IPropertyPhotosHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetAllPropertyPhotosByPropertyRequest
        {
            PropertyId = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetAllByPropertyAsync(request);

        return result.IsSuccess 
            ? Results.Ok(result) 
            : Results.BadRequest(result);
    }
}