using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums;

public class CreateCondominiumEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Condominiums: Create")
            .WithSummary("Cria um novo condomínio")
            .WithDescription("Cria um novo condomínio")
            .WithOrder(1)
            .Produces<Response<Condominium?>>(StatusCodes.Status201Created)
            .Produces<Response<Condominium?>>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICondominiumHandler handler,
        Condominium request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.CreateAsync(request);

        return result.IsSuccess
            ? Results.Created($"/{result.Data?.Id}", result)
            : Results.BadRequest(result);
    }

}
