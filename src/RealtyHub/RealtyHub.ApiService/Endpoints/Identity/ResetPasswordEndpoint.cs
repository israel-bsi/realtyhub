using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class ResetPasswordEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/reset-password", HandlerAsync);
    }

    private static async Task<IResult> HandlerAsync(
        UserManager<User> userManager,
        IEmailService emailService,
        ResetPasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Results.BadRequest(
                new Response<string>(null, 404, "Usuário não encontrado"));

        var decodedToken = WebEncoders.Base64UrlDecode(request.Token);
        var token = Encoding.UTF8.GetString(decodedToken);
        var resetResult = await userManager.ResetPasswordAsync(user, token, request.Password);

        return resetResult.Succeeded ? Results.Ok() : Results.BadRequest(resetResult.Errors);
    }
}