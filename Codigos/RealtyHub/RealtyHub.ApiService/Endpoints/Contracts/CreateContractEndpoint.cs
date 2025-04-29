using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts
{
    /// <summary>
    /// Endpoint responsável por criar novos contratos.
    /// </summary>
    /// <remarks>
    /// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de criação de contratos.
    /// Este endpoint recebe o objeto do contrato e cria um novo registro no banco de dados, associando
    /// o usuário autenticado como criador do contrato.
    /// </remarks>
    public class CreateContractEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para criar um contrato.
        /// </summary>
        /// <remarks>
        /// Registra a rota POST que recebe o objeto <see cref="Contract"/> e invoca o manipulador para criar o contrato.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app)
            => app.MapPost("/", HandlerAsync)
                .WithName("Contracts: Create")
                .WithSummary("Cria um novo contrato")
                .WithDescription("Cria um novo contrato")
                .WithOrder(1)
                .Produces<Response<Contract?>>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para criar um contrato.
        /// </summary>
        /// <remarks>
        /// Este método extrai as informações do contrato do corpo da requisição, associa o ID do usuário autenticado,
        /// e chama o handler para criar o novo contrato. Retorna Created com os dados do contrato criado ou BadRequest em caso de falha.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado.</param>
        /// <param name="handler">Handler responsável pelas operações relacionadas a contratos.</param>
        /// <param name="request">Objeto contendo os dados para criação do contrato.</param>
        /// <returns>
        /// Retorna um objeto do tipo IResult representando a resposta HTTP, com status 201 Created se o contrato for criado com sucesso,
        /// ou 400 BadRequest em caso de erro.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IContractHandler handler,
            Contract request)
        {
            request.UserId = user.Identity?.Name ?? string.Empty;
            var result = await handler.CreateAsync(request);

            return result.IsSuccess
                ? Results.Created($"/{result.Data?.Id}", result)
                : Results.BadRequest(result);
        }
    }
}