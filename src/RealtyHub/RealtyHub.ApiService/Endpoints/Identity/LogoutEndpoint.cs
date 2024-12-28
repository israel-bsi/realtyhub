using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class LogoutEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/logout", HandlerAsync)
            .RequireAuthorization();
    private static async Task<IResult> HandlerAsync(SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
}