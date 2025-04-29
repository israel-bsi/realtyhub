using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts
{
    /// <summary>
    /// Endpoint responsável por atualizar um contrato.
    /// </summary>
    /// <remarks>
    /// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de um contrato.
    /// Este endpoint espera receber o ID do contrato na rota e as novas informações do contrato no corpo da requisição.
    /// </remarks>
    public class UpdateContractEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para atualizar um contrato.
        /// </summary>
        /// <remarks>
        /// Registra a rota PUT que espera um parâmetro numérico (ID) e o objeto <see cref="Contract"/> contendo os
        /// dados atualizados para o contrato. Em caso de operação bem-sucedida, retorna um objeto do tipo
        /// <see cref="Response{Contract?}"/> com os dados atualizados; caso contrário, retorna BadRequest.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPut("/{id:long}", HandlerAsync)
                .WithName("Contracts: Update")
                .WithSummary("Atualiza um contrato")
                .WithDescription("Atualiza um contrato")
                .WithOrder(2)
                .Produces<Response<Contract?>>()
                .Produces(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para atualizar um contrato.
        /// </summary>
        /// <remarks>
        /// Este método extrai o ID do contrato a partir da rota, associa-o ao objeto <see cref="Contract"/> recebido
        /// no corpo da requisição, e define o usuário autenticado responsável pela atualização. Em seguida, invoca o
        /// handler para atualizar os dados no banco e retorna uma resposta HTTP apropriada com base no resultado.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se houver.</param>
        /// <param name="handler">Handler responsável pelas operações de contrato.</param>
        /// <param name="request">Objeto contendo os dados para atualização do contrato.</param>
        /// <param name="id">ID do contrato a ser atualizado.</param>
        /// <returns>
        /// Retorna um objeto do tipo <see cref="IResult"/> contendo:
        /// <list type="bullet">
        /// <item><description>Status OK com os dados atualizados, se a operação for bem-sucedida;</description></item>
        /// <item><description>Status BadRequest com os detalhes do erro, caso contrário.</description></item>
        /// </list>
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IContractHandler handler,
            Contract request,
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