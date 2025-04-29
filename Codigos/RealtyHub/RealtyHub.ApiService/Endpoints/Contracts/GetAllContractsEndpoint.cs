using Microsoft.AspNetCore.Mvc;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Contracts
{
    /// <summary>
    /// Endpoint responsável por recuperar todos os contratos.
    /// </summary>
    /// <remarks>
    /// Implementa a interface <see cref="IEndpoint"/> para mapear a rota que retorna uma lista paginada de contratos.
    /// Este endpoint utiliza parâmetros de consulta para paginação e espera o usuário autenticado para personalizar a requisição.
    /// </remarks>
    public class GetAllContractsEndpoint : IEndpoint
    {
        /// <summary>
        /// Mapeia o endpoint para recuperar todos os contratos.
        /// </summary>
        /// <remarks>
        /// Registra a rota GET para retornar uma lista paginada de contratos. Em caso de sucesso,
        /// retorna um objeto do tipo <see cref="PagedResponse{List{Contract}?}"/>; caso contrário, retorna BadRequest.
        /// </remarks>
        /// <param name="app">O construtor de rotas do aplicativo.</param>
        public static void Map(IEndpointRouteBuilder app)
            => app.MapGet("/", HandlerAsync)
                .WithName("Contracts: Get All")
                .WithSummary("Recupera todos os contratos")
                .WithDescription("Recupera todos os contratos")
                .WithOrder(5)
                .Produces<PagedResponse<List<Contract>?>>()
                .Produces(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Manipulador da rota que recebe a requisição para recuperar todos os contratos.
        /// </summary>
        /// <remarks>
        /// Este método extrai os parâmetros de paginação da consulta, constrói uma requisição para o handler,
        /// e retorna a resposta apropriada. Caso a operação seja bem-sucedida, retorna um status OK com os dados;
        /// caso contrário, retorna BadRequest.
        /// </remarks>
        /// <param name="user">Informações do usuário autenticado.</param>
        /// <param name="handler">Handler responsável pelas operações de contrato.</param>
        /// <param name="pageNumber">Número da página a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        /// <returns>
        /// Retorna um objeto do tipo <see cref="IResult"/> contendo a resposta HTTP com os dados paginados ou erro.
        /// </returns>
        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IContractHandler handler,
            [FromQuery] int pageNumber = Core.Configuration.DefaultPageNumber,
            [FromQuery] int pageSize = Core.Configuration.DefaultPageSize)
        {
            var request = new GetAllContractsRequest
            {
                UserId = user.Identity?.Name ?? string.Empty,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await handler.GetAllAsync(request);

            return result.IsSuccess
                ? Results.Ok(result)
                : Results.BadRequest(result);
        }
    }
}