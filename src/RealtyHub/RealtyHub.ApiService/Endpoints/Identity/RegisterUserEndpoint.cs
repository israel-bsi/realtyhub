using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Requests.Account;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/registeruser", HandlerAsync);
    }

    private static async Task<IResult> HandlerAsync(
        RegisterRequest request, 
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            GivenName = request.GivenName,
            Creci = request.Creci
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        return createResult.Succeeded 
            ? Results.Ok("Usuário registrado com sucesso!") 
            : Results.BadRequest(createResult.Errors);
    }
}