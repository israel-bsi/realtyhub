using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services.Reports;

public class PropertyReportService(List<Property> properties) : IDocument
{
    public DocumentMetadata GetMetadata() => new()
    {
        Title = "Relatório de Imóveis",
        Author = "RealtyHub",
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(9));
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        var pathLogo = Path.Combine(Configuration.LogosPath, "white-logo-nobg.png");
        container.Row(row =>
        {
            row.RelativeItem().Text("Relatório de Imóveis")
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
                columns.ConstantColumn(80);
                columns.ConstantColumn(70);
                columns.RelativeColumn(15);
                columns.RelativeColumn(15);
                columns.RelativeColumn(15);
                columns.RelativeColumn(15);
                columns.RelativeColumn(10);
                columns.RelativeColumn(10);
            });

            table.Header(header =>
            {
                header.Cell().Background("#EEE").Text("Imóvel");
                header.Cell().Background("#EEE").Text("Cadastro");
                header.Cell().Background("#EEE").Text("Última atualização");
                header.Cell().Background("#EEE").Text("Valor");
                header.Cell().Background("#EEE").Text("Quartos");
                header.Cell().Background("#EEE").Text("Banheiros");
                header.Cell().Background("#EEE").Text("Garagem");
                header.Cell().Background("#EEE").Text("Area(m²)");
                header.Cell().Background("#EEE").Text("Novo");
                header.Cell().Background("#EEE").Text("Ativo");
            });

            foreach (var property in properties)
            {
                table.Cell().Text($"{property.Id}");
                table.Cell().Text($"{property.CreatedAt:dd/MM/yyyy}");
                table.Cell().Text($"{property.UpdatedAt:dd/MM/yyyy}");
                table.Cell().Text($"{property.Price:C}");
                table.Cell().Text($"{property.Bedroom}");
                table.Cell().Text($"{property.Bathroom}");
                table.Cell().Text($"{property.Garage}");
                table.Cell().Text($"{property.Area}");
                table.Cell().Text(property.IsNew ? "Sim" : "Não");
                table.Cell().Text(property.IsActive ? "Sim": "Não");
            }
        });
    }
}