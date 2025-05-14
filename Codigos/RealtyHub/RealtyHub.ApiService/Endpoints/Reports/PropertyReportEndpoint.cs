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
/// Endpoint responsável por gerar relatórios de imóveis.
/// </summary>
/// <remarks>
/// Implementa a interface <see cref="IEndpoint"/> para mapear a rota de geração de relatórios de imóveis.
/// </remarks>
public class PropertyReportEndpoint : IEndpoint
{
    /// <summary>
    /// Mapeia o endpoint para gerar relatórios de imóveis.
    /// </summary>
    /// <remarks>
    /// Registra a rota GET que gera um relatório em PDF contendo informações sobre imóveis.
    /// </remarks>
    /// <param name="app">O construtor de rotas do aplicativo <see cref="IEndpointRouteBuilder"/>.</param>
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/property", HandleAsync)
            .WithName("Reports: Property")
            .WithSummary("Relatório de Imóveis")
            .WithDescription("Gera um relatório de imóveis")
            .WithOrder(1)
            .Produces<Response<Report>>()
            .Produces<Response<Report>>(StatusCodes.Status401Unauthorized)
            .Produces<Response<Report>>(StatusCodes.Status500InternalServerError)
            .Produces<Response<Report>>(StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Manipulador da rota que gera o relatório de imóveis.
    /// </summary>
    /// <remarks>
    /// Este método busca os dados de imóveis no banco de dados, gera um relatório em PDF utilizando o serviço de relatórios,
    /// salva o arquivo gerado no servidor e retorna a URL para download do relatório.
    /// </remarks>
    /// <param name="user">Objeto <see cref="ClaimsPrincipal"/> contendo os dados do usuário autenticado.</param>
    /// <param name="dbContext">Contexto do banco de dados <see cref="AppDbContext"/> para buscar os imóveis.</param>
    /// <param name="environment">Ambiente de hospedagem <see cref="IHostEnvironment"/> para acessar configurações do servidor.</param>
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
        IHostEnvironment environment,
        HttpContext httpContext)
    {
        var propertiesData = await dbContext.Properties
            .ToListAsync();

        var report = new PropertyReportService(propertiesData);
        var pdfBytes = report.GeneratePdf();

        var fileName = Path.Combine($"Relatorio Imoveis {DateTime.Now:yyyyMMddHHmmss}.pdf");
        var filePath = Path.Combine(Configuration.ReportsPath, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var reportUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/reports/{fileName}";
        var reportData = new Report { Url = reportUrl };

        return Results.Ok(new Response<Report>(reportData));
    }
}