using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        app.MapPost("/reset-password", HandlerAsync)
            .Produces<Response<string>>()
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandlerAsync(
        UserManager<User> userManager,
        IEmailService emailService,
        ResetPasswordRequest request,
        [FromQuery] string userId,
        [FromQuery] string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.BadRequest(
                new Response<string>(null, 404, "Usuário não encontrado"));

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        var resetResult = await userManager
            .ResetPasswordAsync(user, decodedToken, request.PasswordResetModel.Password);

        return resetResult.Succeeded 
            ? Results.Ok(new Response<string>(null, message: "Senha redefinida com sucesso")) 
            : Results.BadRequest(resetResult.Errors);
    }
}