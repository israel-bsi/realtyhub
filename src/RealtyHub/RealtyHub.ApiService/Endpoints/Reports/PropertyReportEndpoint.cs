using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Endpoints.Reports;

public class PropertyReportEndpoint : IEndpoint
{
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