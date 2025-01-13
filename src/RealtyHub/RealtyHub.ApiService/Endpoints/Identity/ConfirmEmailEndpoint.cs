using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class ConfirmEmailEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("confirm-email", Handler)
            .Produces<Response<bool>>();
    }

    private static async Task<IResult> Handler(
        UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal,
        [FromQuery] string userId, 
        [FromQuery] string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.NotFound();

        var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (emailConfirmed)
            return Results.BadRequest("Email já confirmado!");

        var result = await userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded
            ? Results.Ok()
            : Results.BadRequest(result.Errors);
    }
}