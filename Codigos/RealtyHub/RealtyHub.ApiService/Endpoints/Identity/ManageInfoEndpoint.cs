using Microsoft.AspNetCore.Identity;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Identity;

/// <summary>
/// Endpoint responsável por gerenciar as informações do usuário autenticado.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de obtenção das informações do usuário autenticado.
/// </remarks>
public class ManageInfoEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para obter as informações do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que retorna as informações do usuário autenticado, como nome, email, CRECI e claims.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/manageinfo", HandlerAsync)
            .Produces<UserResponse>()
            .Produces<Response<string>>(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    /// <summary>
    /// Manipulador da rota que retorna as informações do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Este método utiliza o <see cref="UserManager{User}"/> para buscar o usuário autenticado e retorna suas informações.
    /// </remarks>
    /// <param name="userManager">Gerenciador de usuários <see cref="UserManager{User}"/> para realizar operações relacionadas ao usuário.</param>
    /// <param name="claimsPrincipal">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com as informações do usuário, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 401 Unauthorized, se o usuário não estiver autenticado.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal)
    {
        var user = await userManager.GetUserAsync(claimsPrincipal);

        return user is null
            ? Results.Unauthorized()
            : Results.Ok(new UserResponse
            {
                GivenName = user.GivenName,
                Creci = user.Creci,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Claims = claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value)
            });
    }
}