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

/// <summary>
/// Endpoint responsável por redefinir a senha de um usuário.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de redefinição de senha.
/// </remarks>
public class ResetPasswordEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para redefinir a senha de um usuário.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe o ID do usuário, o token de redefinição e a nova senha.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/reset-password", HandlerAsync)
            .Produces<Response<string>>()
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que realiza a redefinição de senha de um usuário.
    /// </summary>
    /// <remarks>
    /// Este método busca o usuário pelo ID, decodifica o token de redefinição de senha,
    /// e redefine a senha do usuário com base nos dados fornecidos.
    /// </remarks>
    /// <param name="userManager">Gerenciador de usuários <see cref="UserManager{User}"/> para realizar operações relacionadas ao usuário.</param>
    /// <param name="emailService">Serviço de email <see cref="IEmailService"/> para envio de notificações, se necessário.</param>
    /// <param name="request">Objeto <see cref="ResetPasswordRequest"/> contendo os dados da nova senha.</param>
    /// <param name="userId">ID do usuário <see cref="string"/> cujo senha será redefinida.</param>
    /// <param name="token">Token de redefinição de senha codificado <see cref="string"/>.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se a senha for redefinida com sucesso;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na redefinição de senha ou o usuário não for encontrado.</para>
    /// </returns>
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