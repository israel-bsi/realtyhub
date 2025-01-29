using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services.Reports;

public class VisitReportService(List<Viewing> viewings, string rootPath) : IDocument
{
    public DocumentMetadata GetMetadata() => new()
    {
        Title = "Relatório de Visitas a Imóveis",
        Author = "RealtyHub",
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
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
            row.RelativeItem().Text("Relatório de Visitas a Imóveis")
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
        // Abre a seção
        container
            .Section("Main")
            .Table(table =>
            {
                // Definir quantas colunas
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50); // Ex: Número do imóvel
                    columns.RelativeColumn();    // Data da visita
                    columns.RelativeColumn();    // Corretor
                    columns.RelativeColumn();    // Feedback
                    columns.RelativeColumn();    // Feedback
                });

                // Cabeçalho da tabela
                table.Header(header =>
                {
                    header.Cell().Background("#EEE").Text("Imóvel");
                    header.Cell().Background("#EEE").Text("Data");
                    header.Cell().Background("#EEE").Text("Status");
                    header.Cell().Background("#EEE").Text("Comprador");
                    header.Cell().Background("#EEE").Text("Vendedor");
                });

                // Linhas com dados
                foreach (var viewing in viewings)
                {
                    table.Cell().Text($"{viewing.PropertyId}");
                    table.Cell().Text($"{viewing.ViewingDate:dd/MM/yyyy}");
                    table.Cell().Text($"{viewing.ViewingStatus.GetDisplayName()}");
                    table.Cell().Text(viewing.Buyer!.Name);
                    table.Cell().Text(viewing.Property!.Seller!.Name);
                }
            });
    }
}