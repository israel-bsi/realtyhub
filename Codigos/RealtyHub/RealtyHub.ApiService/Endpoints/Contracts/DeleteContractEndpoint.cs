using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts
{
    /// <summary>
    /// Endpoint responsável por deletar um contrato.
    /// </summary>
    /// <remarks>
    /// Este endpoint implementa a interface <see cref="IEndpoint"/> e mapeia a rota de exclusão lógica de contratos.
    /// Espera receber o ID do contrato na rota, constrói uma requisição e invoca o handler para deletar o contrato. 
    /// Se a exclusão for bem-sucedida, retorna NoContent; caso contrário, retorna BadRequest com os detalhes do erro.
    /// </remarks>
    public class DeleteContractEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para deletar um contrato.
        /// </summary>
        /// <remarks>
        /// Registra a rota DELETE que espera um parâmetro numérico (ID) na URL e chama o manipulador para executar a operação.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app)
            => app.MapDelete("/{id:long}", HandlerAsync)
                .WithName("Contracts: Delete")
                .WithSummary("Deleta um contrato")
                .WithDescription("Deleta um contrato")
                .WithOrder(3)
                .Produces<Response<Contract?>>(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para deletar um contrato.
        /// </summary>
        /// <remarks>
        /// Este método extrai o ID do contrato da rota, associa o usuário autenticado à requisição,
        /// e invoca o handler para realizar a operação de exclusão lógica do contrato. Retorna NoContent
        /// se a operação for bem-sucedida ou BadRequest em caso de erro.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado, se disponíveis.</param>
        /// <param name="handler">Handler responsável pela operação de exclusão de contratos.</param>
        /// <param name="id">ID do contrato a ser deletado.</param>
        /// <returns>
        /// Retorna um resultado HTTP: NoContent se a exclusão for bem-sucedida ou BadRequest em caso de falha.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IContractHandler handler,
            long id)
        {
            var request = new DeleteContractRequest
            {
                Id = id,
                UserId = user.Identity?.Name ?? string.Empty
            };

            var result = await handler.DeleteAsync(request);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result);
        }
    }
}