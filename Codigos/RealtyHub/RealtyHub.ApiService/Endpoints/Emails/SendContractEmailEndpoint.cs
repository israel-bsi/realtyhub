using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Requests.Emails;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;

namespace RealtyHub.ApiService.Endpoints.Emails;

public class SendContractEmailEndpoint : IEndpoint
{
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