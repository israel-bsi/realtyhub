using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;

namespace RealtyHub.ApiService.Endpoints.Reports;

public class OfferReportEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/offer", HandleAsync)
            .WithName("Reports: Offer")
            .WithSummary("Relatório de Propostas a Imóveis")
            .WithDescription("Gera um relatório de propostas a imóveis")
            .WithOrder(2)
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
        var offersData = await dbContext.Offers
            .Include(o => o.Property)
            .ThenInclude(p => p!.Seller)
            .Include(o => o.Buyer)
            .ToListAsync();

        var basePath = environment.ContentRootPath;
        var report = new OfferReportService(offersData, basePath);
        var pdfBytes = report.GeneratePdf();
        var reportsFolder = Path.Combine(basePath, "Sources", "Reports");
        if (!Directory.Exists(reportsFolder))
            Directory.CreateDirectory(reportsFolder);

        var fileName = Path.Combine($"RelatórioPropostas{DateTime.Now:yyyyMMddHHmmss}.pdf");
        var filePath = Path.Combine(reportsFolder, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);

        var reportUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/reports/{fileName}";

        return Results.Ok(new { url = reportUrl });
    }
}