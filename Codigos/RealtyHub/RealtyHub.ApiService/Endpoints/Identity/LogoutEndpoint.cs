using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;

namespace RealtyHub.ApiService.Endpoints.Identity;

/// <summary>
/// Endpoint responsável por realizar o logout de um usuário autenticado.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de logout.
/// </remarks>
public class LogoutEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para realizar o logout.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que realiza o logout do usuário autenticado.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/logout", HandlerAsync)
            .RequireAuthorization();

    /// <summary>
    /// Manipulador da rota que realiza o logout do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Este método utiliza o <see cref="SignInManager{User}"/> para realizar o logout do usuário atual.
    /// </remarks>
    /// <param name="signInManager">Gerenciador de autenticação <see cref="SignInManager{User}"/>.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se o logout for realizado com sucesso.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
}