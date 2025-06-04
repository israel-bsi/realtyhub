using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;
using System.Text;

namespace RealtyHub.ApiService.Endpoints.Identity;

/// <summary>
/// Endpoint responsável por confirmar o e-mail de um usuário.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de confirmação de e-mail.
/// </remarks>
public class ConfirmEmailEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para confirmar o e-mail de um usuário.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que recebe o ID do usuário e o token de confirmação como parâmetros de consulta
    /// e chama o manipulador para confirmar o e-mail.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("confirm-email", Handler)
            .Produces<Response<string>>()
            .Produces<Response<string>>(StatusCodes.Status404NotFound)
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que recebe a requisição para confirmar o e-mail de um usuário.
    /// </summary>
    /// <remarks>
    /// Este método busca o usuário pelo ID, verifica se o e-mail já foi confirmado e, caso contrário,
    /// decodifica o token de confirmação e chama o serviço de confirmação de e-mail.
    /// </remarks>
    /// <param name="userManager">Gerenciador de usuários <see cref="UserManager{User}"/> para realizar operações relacionadas ao usuário.</param>
    /// <param name="claimsPrincipal">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="userId">ID do usuário a ser confirmado <see cref="string"/>.</param>
    /// <param name="token">Token de confirmação codificado <see cref="string"/>.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se o e-mail for confirmado com sucesso;</para>
    /// <para>- HTTP 400 Bad Request, se o e-mail já estiver confirmado ou o token for inválido;</para>
    /// <para>- HTTP 404 Not Found, se o usuário não for encontrado.</para>
    /// </returns>
    private static async Task<IResult> Handler(
        UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal,
        [FromQuery] string userId,
        [FromQuery] string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.NotFound(new Response<string>(null, 400,
                "Usuário não encontrado!"));

        var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (emailConfirmed)
            return Results.BadRequest(new Response<string>(null, 400,
                "Email já confirmado!"));

        var decodedBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedBytes);
        var result = await userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded
            ? Results.Ok(new Response<string>(null, 200,
                "Email confirmado com sucesso!"))
            : Results.BadRequest(result.Errors);
    }
}