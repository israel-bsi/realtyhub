using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums
{
    /// <summary>
    /// Endpoint responsável por criar novos condomínios.
    /// </summary>
    /// <remarks>
    /// Implementa <see cref="IEndpoint"/> para mapear a rota de criação de condomínios.
    /// </remarks>
    public class CreateCondominiumEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para criar um condomínio.
        /// </summary>
        /// <remarks>
        /// Este método é chamado para registrar o endpoint no aplicativo.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandlerAsync)
                .WithName("Condominiums: Create")
                .WithSummary("Cria um novo condomínio")
                .WithDescription("Cria um novo condomínio")
                .WithOrder(1)
                .Produces<Response<Condominium?>>(StatusCodes.Status201Created)
                .Produces<Response<Condominium?>>(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para criar um condomínio.
        /// </summary>
        /// <remarks>
        /// Este método é responsável por processar a requisição e retornar o resultado.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se houver.</param>
        /// <param name="handler">Handler responsável pelas operações de condomínio.</param>
        /// <param name="request">Objeto que contém as informações para criar o condomínio.</param>
        /// <returns>Retorna o resultado da criação do condomínio ou um erro.</returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            ICondominiumHandler handler,
            Condominium request)
        {
            request.UserId = user.Identity?.Name ?? string.Empty;
            var result = await handler.CreateAsync(request);

            return result.IsSuccess
                ? Results.Created($"/{result.Data?.Id}", result)
                : Results.BadRequest(result);
        }
    }
}