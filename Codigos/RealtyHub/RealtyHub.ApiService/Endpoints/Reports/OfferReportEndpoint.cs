using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Reports;

/// <summary>
/// Endpoint responsável por gerar relatórios de propostas a imóveis.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de geração de relatórios de propostas.
/// </remarks>
public class OfferReportEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para gerar relatórios de propostas a imóveis.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que gera um relatório em PDF contendo informações sobre propostas a imóveis.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/offer", HandleAsync)
            .WithName("Reports: Offer")
            .WithSummary("Relatório de Propostas a Imóveis")
            .WithDescription("Gera um relatório de propostas a imóveis")
            .WithOrder(2)
            .Produces<Response<Report>>()
            .Produces<Response<Report>>(StatusCodes.Status401Unauthorized)
            .Produces<Response<Report>>(StatusCodes.Status500InternalServerError)
            .Produces<Response<Report>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que gera o relatório de propostas a imóveis.
    /// </summary>
    /// <remarks>
    /// Este método busca os dados de propostas no banco de dados, gera um relatório em PDF utilizando o serviço de relatórios,
    /// salva o arquivo gerado no servidor e retorna a URL para download do relatório.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="dbContext">Contexto do banco de dados <see cref="AppDbContext"/> para buscar as propostas.</param>
    /// <param name="httpContext">Contexto HTTP <see cref="HttpContext"/> para construir a URL do relatório.</param>
    /// <returns>
    /// Um objeto <see cref="IResult"/> representando a resposta HTTP:
    /// <para>- HTTP 200 OK com a URL do relatório, se a operação for bem-sucedida;</para>
    /// <para>- HTTP 400 Bad Request, se houver erros na requisição;</para>
    /// <para>- HTTP 401 Unauthorized, se o usuário não estiver autenticado;</para>
    /// <para>- HTTP 500 Internal Server Error, se ocorrer um erro inesperado.</para>
    /// </returns>
    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        AppDbContext dbContext,
        HttpContext httpContext)
    {
        var offersData = await dbContext.Offers
            .Include(o => o.Imovel)
            .ThenInclude(p => p!.Vendedor)
            .Include(o => o.Comprador)
            .ToListAsync();

        var report = new OfferReportService(offersData);
        var pdfBytes = report.GeneratePdf();

        var fileName = Path.Combine($"Relatorio Propostas {DateTime.Now:yyyyMMddHHmmss}.pdf");
        var filePath = Path.Combine(Configuration.ReportsPath, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var reportUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/reports/{fileName}";

        var reportData = new Report { Url = reportUrl };

        return Results.Ok(new Response<Report>(reportData));
    }
}