using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services.Reports;

public class OfferReportService(List<Offer> offers, string rootPath) : IDocument
{
    public DocumentMetadata GetMetadata() => new()
    {
        Title = "Relatório de Propostas a Imóveis",
        Author = "RealtyHub",
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.DefaultTextStyle(x=>x.FontSize(8));
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        var pathLogo = Path.Combine(rootPath, "Sources", "Logos", "white-logo-nobg.png");
        container.Row(row =>
        {
            row.RelativeItem().Text("Relatório de Propostas a Imóveis")
                .FontSize(16)
                .Bold();

            row.ConstantItem(100).Height(50)
                .Image(pathLogo);
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(txt =>
        {
            txt.CurrentPageNumber();
            txt.Span(" / ");
            txt.TotalPages();
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Section("Main").Table(table =>
        {

            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50);
                columns.ConstantColumn(70);
                columns.ConstantColumn(60);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Background("#EEE").Text("Proposta");
                header.Cell().Background("#EEE").Text("Data");
                header.Cell().Background("#EEE").Text("Status");
                header.Cell().Background("#EEE").Text("Valor proposto");
                header.Cell().Background("#EEE").Text("Comprador");
                header.Cell().Background("#EEE").Text("Vendedor");
            });

            foreach (var offer in offers)
            {
                table.Cell().Text($"{offer.PropertyId}");
                table.Cell().Text($"{offer.SubmissionDate:dd/MM/yyyy}");
                table.Cell().Text($"{offer.OfferStatus.GetDisplayName()}");
                table.Cell().Text(offer.Amount.ToString("C"));
                table.Cell().Text(offer.Buyer!.Name);
                table.Cell().Text(offer.Property!.Seller!.Name);
            }
        });
    }
}