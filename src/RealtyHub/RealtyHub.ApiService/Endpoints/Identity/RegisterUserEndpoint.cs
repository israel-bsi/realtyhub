using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Requests.Account;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Services;

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
            Creci = request.Creci,
            VerificationCode = Guid.NewGuid().ToString("N")[..6].ToUpper()
        };

        var createResult = await userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
            return Results.BadRequest(createResult.Errors);

        var claims = new List<Claim>
        {
            new("VerificationCode", user.VerificationCode),
            new("Creci", user.Creci),
            new (ClaimTypes.GivenName, user.GivenName)
        };

        var addClaimsResult = await userManager.AddClaimsAsync(user, claims);
        if (!addClaimsResult.Succeeded)
            return Results.BadRequest(addClaimsResult.Errors);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{Core.Configuration.FrontendUrl}" +
                               $"/confirmar-email?" +
                               $"userId={user.Id}" +
                               $"&token={Uri.EscapeDataString(token)}";

        var emailRequest = new EmailMessageRequest
        {
            EmailTo = user.Email,
            Name = user.GivenName,
            Code = user.VerificationCode,
            ConfirmationLink = confirmationLink
        };

        var emailResult = await emailService.SendVerificationCodeAsync(emailRequest);

        return emailResult.IsSuccess 
            ? Results.Ok("Usuário registrado com sucesso! Verifique seu e-mail!") 
            : Results.BadRequest(addClaimsResult.Errors);
    }
}