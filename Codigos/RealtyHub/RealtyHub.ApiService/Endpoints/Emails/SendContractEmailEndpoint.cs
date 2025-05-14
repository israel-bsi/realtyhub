using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Endpoints.Emails;

/// <summary>
/// Endpoint responsável por enviar um email contendo um contrato como anexo.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de envio de emails com contratos.
/// </remarks>
public class SendContractEmailEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para enviar um email com o contrato.
    /// </summary>
    /// <remarks>
    /// Registra a rota POST que recebe os dados do email e do contrato e chama o manipulador para enviar o email.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/contract", HandlerAsync)
            .WithName("Emails: SendContract")
            .WithSummary("Envia um email com o contrato")
            .WithDescription("Envia um email com o contrato")
            .WithOrder(1)
            .Produces(StatusCodes.Status200OK)
            .Produces<Response<string>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que recebe a requisição para enviar um email com o contrato.
    /// </summary>
    /// <remarks>
    /// Este método busca o contrato no banco de dados, verifica sua existência, gera o caminho do anexo
    /// e chama o serviço de email para enviar o contrato. Retorna uma resposta apropriada com base no resultado.
    /// </remarks>
    /// <param name="emailService">Instância de <see cref="IEmailService"/> responsável pelo envio de emails.</param>
    /// <param name="request">Objeto <see cref="AttachmentMessage"/> contendo os dados do email e do contrato.</param>
    /// <param name="context">Instância de <see cref="AppDbContext"/> para acesso ao banco de dados.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK, se o email for enviado com sucesso;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição ou o contrato não for encontrado.</para>
    /// </returns>
    private static async Task<IResult> HandlerAsync(
        IEmailService emailService,
        AttachmentMessage request,
        AppDbContext context)
    {
        var contract = await context
            .Contracts
            .FirstOrDefaultAsync(c => c.Id == request.ContractId);

        if (contract is null)
            return Results.BadRequest(new Response<string>(null, 404, "Contrato não encontrado"));

        var attachmentPath = Path.Combine(Configuration.ContractsPath, $"{contract.FileId}.pdf");

        request.AttachmentPath = attachmentPath;

        var result = await emailService.SendContractAsync(request);

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(result);
    }
}