using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;

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
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        AppDbContext dbContext,
        IHostEnvironment environment,
        HttpContext httpContext)
    {
        var propertiesData = await dbContext.Properties
            .ToListAsync();

        var basePath = environment.ContentRootPath;
        var report = new PropertyReportService(propertiesData, basePath);
        var pdfBytes = report.GeneratePdf();
        var reportsFolder = Path.Combine(basePath, "Sources", "Reports");
        if (!Directory.Exists(reportsFolder))
            Directory.CreateDirectory(reportsFolder);

        var fileName = Path.Combine($"RelatórioImóveis{DateTime.Now:yyyyMMddHHmmss}.pdf");
        var filePath = Path.Combine(reportsFolder, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var reportUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/reports/{fileName}";

        return Results.Ok(new { url = reportUrl });
    }
}