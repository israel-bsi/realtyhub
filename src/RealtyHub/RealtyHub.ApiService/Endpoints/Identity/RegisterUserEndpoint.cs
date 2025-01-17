using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Security.Claims;
using System.Text;

namespace RealtyHub.ApiService.Endpoints.Identity;

public class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/registeruser", HandlerAsync);
    }

    private static async Task<IResult> HandlerAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        IEmailService emailService,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            GivenName = request.GivenName,
            Creci = request.Creci
        };

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
            return Results.BadRequest(createResult.Errors);

        var claims = new List<Claim>
        {
            new("Creci", user.Creci),
            new (ClaimTypes.GivenName, user.GivenName)
        };

        var addClaimsResult = await userManager.AddClaimsAsync(user, claims);
        if (!addClaimsResult.Succeeded)
            return Results.BadRequest(addClaimsResult.Errors);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationLink = $"{Core.Configuration.FrontendUrl}" +
                               $"/confirmar-email?" +
                               $"userId={user.Id}" +
                               $"&token={encodedToken}";

        var emailRequest = new EmailMessageRequest
        {
            EmailTo = user.Email,
            Name = user.GivenName,
            ConfirmationLink = confirmationLink
        };

        var emailResult = await emailService.SendConfirmationLinkAsync(emailRequest);

        return emailResult.IsSuccess
            ? Results.Ok(new Response<string>(null, message: "Usuário registrado com sucesso! Verifique seu e-mail!"))
            : Results.BadRequest(emailResult.Message);
    }
}