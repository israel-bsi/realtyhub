using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services.Reports;

/// <summary>
/// Serviço que gera um relatório de propostas a imóveis utilizando a biblioteca QuestPDF.
/// </summary>
public class OfferReportService : IDocument
{
    /// <summary>
    /// Lista de ofertas a serem incluídas no relatório.
    /// </summary>
    private readonly List<Proposta> _offers;

    /// <summary>
    /// Inicializa uma nova instância de <c><see cref="OfferReportService"/></c> com a lista de ofertas especificada.
    /// </summary>
    /// <param name="offers">A lista de ofertas para inclusão no relatório.</param>
    public OfferReportService(List<Proposta> offers)
    {
        _offers = offers;
    }

    /// <summary>
    /// Obtém os metadados do documento.
    /// </summary>
    /// <remarks>
    /// Os metadados deste documento permitem identificar o título e a autoria do relatório gerado.
    /// </remarks>
    /// <returns>
    /// Um objeto <c><see cref="DocumentMetadata"/></c> contendo o título e o autor do relatório.
    /// </returns>
    public DocumentMetadata GetMetadata() => new()
    {
        Title = "Relatório de Propostas a Imóveis",
        Author = "RealtyHub",
    };

    /// <summary>
    /// Compoe o documento configurando as seções de cabeçalho, conteúdo e rodapé.
    /// </summary>
    /// <param name="container">O contêiner do documento a ser configurado.</param>
    /// <remarks>
    /// Este método organiza o layout do relatório utilizando as funcionalidades da biblioteca QuestPDF.
    /// </remarks>
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(8));
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    /// <summary>
    /// Compoe o cabeçalho do relatório.
    /// </summary>
    /// <param name="container">O contêiner onde o cabeçalho será construído.</param>
    /// <remarks>
    /// O cabeçalho inclui o título do relatório e o logotipo da aplicação.
    /// </remarks>
    private void ComposeHeader(IContainer container)
    {
        var pathLogo = Path.Combine(Configuration.LogosPath, "white-logo-nobg.png");
        container.Row(row =>
        {
            row.RelativeItem().Text("Relatório de Propostas a Imóveis")
                .FontSize(16)
                .Bold();

            row.ConstantItem(100).Height(50)
                .Image(pathLogo);
        });
    }

    /// <summary>
    /// Compoe o rodapé do relatório, exibindo o número da página atual e o total de páginas.
    /// </summary>
    /// <param name="container">O contêiner onde o rodapé será construído.</param>
    /// <remarks>
    /// O rodapé é centralizado e atualiza dinamicamente o número atual e o total de páginas.
    /// </remarks>
    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(txt =>
        {
            txt.CurrentPageNumber();
            txt.Span(" / ");
            txt.TotalPages();
        });
    }

    /// <summary>
    /// Compoe o conteúdo principal do relatório, montando uma tabela com informações das ofertas.
    /// </summary>
    /// <param name="container">O contêiner onde o conteúdo será construído.</param>
    /// <remarks>
    /// Este método cria uma tabela que exibe dados importantes de cada oferta, como identificação, data, status, valor, comprador e vendedor.
    /// </remarks>
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

            foreach (var offer in _offers)
            {
                table.Cell().Text($"{offer.ImovelId}");
                table.Cell().Text($"{offer.DataDeEnvio:dd/MM/yyyy}");
                table.Cell().Text($"{offer.StatusProposta.GetDisplayName()}");
                table.Cell().Text(offer.Valor.ToString("C"));
                table.Cell().Text(offer.Comprador!.Nome);
                table.Cell().Text(offer.Imovel!.Vendedor!.Nome);
            }
        });
    }
}