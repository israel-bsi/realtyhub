using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using RealtyHub.ApiService.Common.Api;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services.Reports;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

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
            .Produces<Response<Report>>()
            .Produces<Response<Report>>(StatusCodes.Status401Unauthorized)
            .Produces<Response<Report>>(StatusCodes.Status500InternalServerError)
            .Produces<Response<Report>>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        AppDbContext dbContext,
        HttpContext httpContext)
    {
        var offersData = await dbContext.Offers
            .Include(o => o.Property)
            .ThenInclude(p => p!.Seller)
            .Include(o => o.Buyer)
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