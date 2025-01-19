using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class ForgotPasswordEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/forgot-password", HandlerAsync);
    }

    private static async Task<IResult> HandlerAsync(
        UserManager<User> userManager,
        IEmailService emailService,
        ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Results.BadRequest(
                new Response<string>(null, 404, "Usuário não encontrado"));

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var resetLink = $"{Core.Configuration.FrontendUrl}" +
                        $"/recuperar-senha?" +
                        $"userId={user.Id}" +
                        $"&token={encodedToken}";
        var message = new ResetPasswordMessage
        {
            EmailTo = user.Email!,
            Name = user.GivenName,
            ResetPasswordLink = resetLink
        };
        await emailService.SendResetPasswordLinkAsync(message);
        return Results.Ok(new Response<string>(null, message: "Verifique seu emaiL!"));
    }
}