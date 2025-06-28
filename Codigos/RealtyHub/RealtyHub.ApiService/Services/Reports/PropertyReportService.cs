using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services.Reports;

/// <summary>
/// Serviço que gera um relatório de imóveis utilizando a biblioteca QuestPDF.
/// </summary>
public class PropertyReportService : IDocument
{
    /// <summary>
    /// Lista de imóveis a serem incluídos no relatório.
    /// </summary>
    private readonly List<Imovel> _properties;

    /// <summary>
    /// Inicializa uma nova instância de <c><see cref="PropertyReportService"/></c> com a lista de imóveis especificada.
    /// </summary>
    /// <remarks>
    /// Este construtor recebe a lista de imóveis que será utilizada para gerar o relatório.
    /// </remarks>
    /// <param name="properties">A lista de imóveis para inclusão no relatório.</param>
    public PropertyReportService(List<Imovel> properties)
    {
        _properties = properties;
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
        Title = "Relatório de Imóveis",
        Author = "RealtyHub",
    };

    /// <summary>
    /// Compoe o documento configurando suas seções de cabeçalho, conteúdo e rodapé.
    /// </summary>
    /// <remarks>
    /// Este método organiza o layout do relatório utilizando as funcionalidades da biblioteca QuestPDF.
    /// </remarks>
    /// <param name="container">O contêiner do documento a ser configurado.</param>
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

    /// <summary>
    /// Compoe o cabeçalho do relatório.
    /// </summary>
    /// <remarks>
    /// O cabeçalho inclui o título do relatório e o logotipo da aplicação.
    /// </remarks>
    /// <param name="container">O contêiner onde o cabeçalho será construído.</param>
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

    /// <summary>
    /// Compoe o rodapé do relatório, exibindo o número da página atual juntamente com o total de páginas.
    /// </summary>
    /// <remarks>
    /// O rodapé é centralizado e atualiza dinamicamente o número da página atual e o total de páginas.
    /// </remarks>
    /// <param name="container">O contêiner onde o rodapé será construído.</param>
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
    /// Compoe o conteúdo principal do relatório, montando uma tabela com as informações dos imóveis.
    /// </summary>
    /// <remarks>
    /// Este método cria uma tabela que exibe dados importantes de cada imóvel, como o identificador, datas de cadastro e atualização, 
    /// preço, quantidade de quartos, banheiros, vagas, área e os status de novo e ativo.
    /// </remarks>
    /// <param name="container">O contêiner onde o conteúdo será construído.</param>
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
                header.Cell().Background("#EEE").Text("Área(m²)");
                header.Cell().Background("#EEE").Text("Novo");
                header.Cell().Background("#EEE").Text("Ativo");
            });

            foreach (var property in _properties)
            {
                table.Cell().Text($"{property.Id}");
                table.Cell().Text($"{property.CriadoEm:dd/MM/yyyy}");
                table.Cell().Text($"{property.AtualizadoEm:dd/MM/yyyy}");
                table.Cell().Text($"{property.Preco:C}");
                table.Cell().Text($"{property.Quarto}");
                table.Cell().Text($"{property.Banheiro}");
                table.Cell().Text($"{property.Garagem}");
                table.Cell().Text($"{property.Area}");
                table.Cell().Text(property.Novo ? "Sim" : "Não");
                table.Cell().Text(property.Ativo ? "Sim" : "Não");
            }
        });
    }
}