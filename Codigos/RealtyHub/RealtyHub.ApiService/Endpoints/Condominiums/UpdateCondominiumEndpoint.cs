using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums
{
    /// <summary>
    /// Endpoint responsável por atualizar um condomínio.
    /// </summary>
    /// <remarks>
    /// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de dados de um condomínio.
    /// Este endpoint espera receber o ID do condomínio na rota e as novas informações no corpo da requisição.
    /// </remarks>
    public class UpdateCondominiumEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para atualizar um condomínio.
        /// </summary>
        /// <remarks>
        /// Registra a rota PUT que espera um parâmetro numérico (ID) e o objeto Condominium com os dados para atualização.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapPut("/{id:long}", HandlerAsync)
                .WithName("Condominiums: Update")
                .WithSummary("Atualiza um condomínio")
                .WithDescription("Atualiza os dados de um condomínio existente")
                .WithOrder(2)
                .Produces<Response<Condominium?>>()
                .Produces<Response<Condominium?>>(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para atualizar um condomínio.
        /// </summary>
        /// <remarks>
        /// Este método extrai o ID do condomínio a partir da rota, associa-o ao objeto Condominium recebido
        /// e chama o handler para atualizar os dados no banco. Retorna OK caso a atualização seja bem-sucedida
        /// ou BadRequest em caso de falha.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se houver.</param>
        /// <param name="handler">Handler responsável pelas operações de condomínio.</param>
        /// <param name="request">Objeto contendo as informações para atualização do condomínio.</param>
        /// <param name="id">ID do condomínio a ser atualizado.</param>
        /// <returns>
        /// Retorna um objeto do tipo IResult contendo a resposta HTTP adequada,
        /// OK com os dados atualizados ou BadRequest em caso de falha.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            ICondominiumHandler handler,
            Condominium request,
            long id)
        {
            request.Id = id;
            request.UserId = user.Identity?.Name ?? string.Empty;

            var result = await handler.UpdateAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);
        }
    }
}