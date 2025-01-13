using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class ManageInfoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/manageinfo", HandlerAsync)
            .Produces<UserResponse>();
    }
    private static async Task<IResult> HandlerAsync(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        return user is null
            ? Results.Unauthorized()
            : Results.Ok(new UserResponse
            {
                GivenName = user.GivenName,
                Creci = user.Creci,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty
            });
    }
}