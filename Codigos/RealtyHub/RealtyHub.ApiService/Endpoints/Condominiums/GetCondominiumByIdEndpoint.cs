using System.Security.Claims;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Condominiums
{
    /// <summary>
    /// Endpoint responsável por recuperar um condomínio específico pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Implementa a interface <see cref="IEndpoint"/> e mapeia a rota para obter um condomínio
    /// baseado no ID fornecido. Caso o condomínio seja encontrado, retorna os detalhes do mesmo;
    /// caso contrário, retorna uma resposta de não encontrado.
    /// </remarks>
    public class GetCondominiumByIdEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para recuperar um condomínio pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Registra a rota GET que espera um parâmetro numérico (ID) e chama o manipulador para
        /// retornar o condomínio correspondente.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app) =>
            app.MapGet("/{id:long}", HandlerAsync)
                .WithName("Condominiums: Get by id")
                .WithSummary("Recupera um condomínio")
                .WithDescription("Recupera um condomínio baseado no seu ID")
                .WithOrder(3)
                .Produces<Response<Condominium?>>()
                .Produces<Response<Condominium?>>(StatusCodes.Status404NotFound);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para recuperar um condomínio pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Este método extrai o ID do condomínio da rota, constrói uma requisição para o handler e
        /// retorna a resposta apropriada. Caso o condomínio seja encontrado, retorna OK com os dados;
        /// caso contrário, retorna NotFound.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se houver.</param>
        /// <param name="handler">Handler responsável pelas operações relativas a condomínios.</param>
        /// <param name="id">ID do condomínio a ser recuperado.</param>
        /// <returns>
        /// Retorna um resultado HTTP com os detalhes do condomínio se encontrado; caso contrário,
        /// retorna NotFound.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            ICondominiumHandler handler,
            long id)
        {
            var request = new GetCondominiumByIdRequest
            {
                Id = id,
                UserId = user.Identity?.Name ?? string.Empty
            };

            var result = await handler.GetByIdAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.NotFound(result);
        }
    }
}