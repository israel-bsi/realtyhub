using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;
using System.Text;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class ConfirmEmailEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("confirm-email", Handler)
            .Produces<Response<string>>();
    }

    private static async Task<IResult> Handler(
        UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal,
        [FromQuery] string userId,
        [FromQuery] string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.NotFound(new Response<string>(null, 400, "Usuário não encontrado!"));

        var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (emailConfirmed)
            return Results.BadRequest(new Response<string>(null, 400, "Email já confirmado!"));

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);
        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded
            ? Results.Ok(new Response<string>(null, 201, "Email confirmado com sucesso!"))
            : Results.BadRequest(result.Errors);
    }
}