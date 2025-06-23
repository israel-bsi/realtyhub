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

/// <summary>
/// Endpoint responsável por iniciar o processo de recuperação de senha de um usuário.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de solicitação de recuperação de senha.
/// </remarks>
public class ForgotPasswordEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para solicitar a recuperação de senha.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe o e-mail do usuário e envia um link de redefinição de senha.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/forgot-password", HandlerAsync)
            .Produces<Response<string>>()
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que recebe a requisição para recuperação de senha.
    /// </summary>
    /// <remarks>
    /// Este método busca o usuário pelo e-mail fornecido, gera um token de redefinição de senha,
    /// cria um link de redefinição e envia o link por e-mail.
    /// </remarks>
    /// <param name="userManager">Gerenciador de usuários <see cref="UserManager{User}"/> para realizar operações relacionadas ao usuário.</param>
    /// <param name="emailService">Serviço de e-mail <see cref="IEmailService"/> responsável por enviar o link de redefinição de senha.</param>
    /// <param name="request">Objeto <see cref="ForgotPasswordRequest"/> contendo o e-mail do usuário.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se o link de redefinição de senha for enviado com sucesso;</para>
    /// <para>- HTTP 400 Bad Request, se o usuário não for encontrado.</para>
    /// </returns>
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
            ResetPasswordLink = resetLink
        };
        await emailService.SendResetPasswordLinkAsync(message);
        return Results.Ok(new Response<string>(null, message: "Verifique seu email!"));
    }
}