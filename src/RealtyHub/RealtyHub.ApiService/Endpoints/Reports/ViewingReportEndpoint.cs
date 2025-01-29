using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;

namespace RealtyHub.ApiService.Endpoints.Reports;

public class ViewingReportEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/viewing", HandleAsync)
            .WithName("Reports: Viewing")
            .WithSummary("Relatório de Visitas a Imóveis")
            .WithDescription("Gera um relatório de visitas a imóveis")
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
        var visitsData = await dbContext.Viewing
            .Include(v => v.Property)
            .ThenInclude(p => p!.Seller)
            .Include(v => v.Buyer)
            .ToListAsync();
        var basePath = environment.ContentRootPath;
        var report = new VisitReportService(visitsData, basePath);
        var pdfBytes = report.GeneratePdf();
        var reportsFolder = Path.Combine(basePath, "Sources", "Reports");
        if(!Directory.Exists(reportsFolder))
            Directory.CreateDirectory(reportsFolder);

        var fileName = Path.Combine($"RelatórioVisitas{DateTime.Now:yyyyMMddHHmmss}.pdf");
        var filePath = Path.Combine(reportsFolder, fileName);

        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var reportUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/reports/{fileName}";

        return Results.Ok(new { url = reportUrl });
    }
}