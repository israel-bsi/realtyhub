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

/// <summary>
/// Endpoint responsável por registrar um novo usuário.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de registro de usuários.
/// </remarks>
public class RegisterUserEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para registrar um novo usuário.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados do usuário e cria uma nova conta.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/register-user", HandlerAsync)
            .Produces<Response<string>>()
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que registra um novo usuário.
    /// </summary>
    /// <remarks>
    /// Este método cria um novo usuário com base nos dados fornecidos, adiciona claims ao usuário,
    /// gera um token de confirmação de e-mail e envia o link de confirmação para o e-mail do usuário.
    /// </remarks>
    /// <param name="request">Objeto <see cref="RegisterRequest"/> contendo os dados do usuário a ser registrado.</param>
    /// <param name="userManager">Gerenciador de usuários <see cref="UserManager{User}"/> para realizar operações relacionadas ao usuário.</param>
    /// <param name="emailService">Serviço de e-mail <see cref="IEmailService"/> responsável por enviar o link de confirmação de e-mail.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se o usuário for registrado com sucesso e o e-mail de confirmação enviado;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na criação do usuário ou no envio do e-mail.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        RegisterRequest request,
        UserManager<User> userManager,
        IEmailService emailService)
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
            new(ClaimTypes.GivenName, user.GivenName)
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

        var emailMessage = new ConfirmEmailMessage
        {
            EmailTo = user.Email,
            ConfirmationLink = confirmationLink
        };

        var emailResult = await emailService.SendConfirmationLinkAsync(emailMessage);

        return emailResult.IsSuccess
            ? Results.Ok(new Response<string>(null,
                message: "Usuário registrado com sucesso! Verifique seu e-mail!"))
            : Results.BadRequest(emailResult.Message);
    }
}