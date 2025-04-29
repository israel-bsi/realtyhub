using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums
{
    /// <summary>
    /// Endpoint responsável por deletar um condomínio.
    /// </summary>
    /// <remarks>
    /// Implementa <see cref="IEndpoint"/> para mapear a rota de exclusão lógica de condomínios.
    /// </remarks>
    public class DeleteCondominiumEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para deletar um condomínio.
        /// </summary>
        /// <remarks>
        /// Registra a rota para receber um ID que identifica o condomínio a ser excluído.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapDelete("/{id:long}", HandlerAsync)
                .WithName("Condominiums: Delete")
                .WithSummary("Deleta um condomínio")
                .WithDescription("Deleta um condomínio")
                .WithOrder(4)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<Response<Condominium?>>(StatusCodes.Status404NotFound);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para deletar um condomínio.
        /// </summary>
        /// <remarks>
        /// Este método realiza a exclusão lógica do condomínio, definindo-o como inativo no banco de dados.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se houver.</param>
        /// <param name="handler">Handler responsável pelas operações de condomínio.</param>
        /// <param name="id">ID do condomínio a ser marcado como inativo.</param>
        /// <returns>
        /// Retorna NoContent se a exclusão for bem-sucedida ou NotFound se o condomínio não for encontrado.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            ICondominiumHandler handler,
            long id)
        {
            var request = new DeleteCondominiumRequest
            {
                Id = id,
                UserId = user.Identity?.Name ?? string.Empty
            };
            var result = await handler.DeleteAsync(request);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.NotFound(result);
        }
    }
}