using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Common.Validation;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using System.Security.Claims;

namespace RealtyHub.ApiService.Endpoints.Customers;

/// <summary>
/// Endpoint responsável por atualizar os dados de um cliente existente.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de atualização de clientes.
/// Aplica validação de Data Annotations conforme definido no modelo Customer.
/// </remarks>
public class UpdateCustomerEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para atualizar um cliente.
    /// </summary>
    /// <remarks>
    /// Registra a rota PUT que espera um parâmetro numérico (ID) e os dados atualizados do cliente,
    /// chamando o manipulador para executar a operação. Aplica validação de Data Annotations.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:long}", HandlerAsync)
            .WithName("Customers: Update")
            .WithSummary("Atualiza um cliente")
            .WithDescription("Atualiza um cliente com validação de Data Annotations")
            .WithOrder(2)
            .Produces<Response<Customer?>>()
            .Produces(StatusCodes.Status400BadRequest);

    /// <summary>
    /// Manipulador da rota que recebe a requisição para atualizar um cliente.
    /// </summary>
    /// <remarks>
    /// Este método:
    /// 1. Valida os dados do cliente usando APENAS os Data Annotations definidos no modelo
    /// 2. Verifica a consistência do ID
    /// 3. Associa o ID do usuário autenticado à requisição
    /// 4. Chama o handler para realizar a atualização
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="handler">Instância de <see cref="ICustomerHandler"/> responsável pelas operações relacionadas a clientes.</param>
    /// <param name="request">Objeto <see cref="Customer"/> contendo os dados atualizados do cliente.</param>
    /// <param name="id">ID do cliente a ser atualizado.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com os dados atualizados do cliente, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request com erros de validação dos Data Annotations.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICustomerHandler handler,
        Customer request,
        long id)
    {
        // Validação APENAS dos Data Annotations
        var validationResult = DataAnnotationValidator.ValidateRecursively(request);
        if (validationResult != null)
        {
            return validationResult; // Retorna BadRequest com erros dos Data Annotations
        }

        // Validação básica de consistência do ID
        if (request.Id != 0 && request.Id != id)
        {
            return Results.BadRequest(new
            {
                Message = "ID do cliente na URL não coincide com o ID no corpo da requisição",
                IsSuccess = false,
                Code = 400
            });
        }

        // Processamento normal
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.UpdateAsync(request);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(result);
    }
}