using System.IO.Compression;
using GrapeCity.Documents.Word;
using GrapeCity.Documents.Word.Layout;
using RealtyHub.Core.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace RealtyHub.ApiService.Services;

/// <summary>
/// Classe responsável por gerar contratos utilizando templates e mapeamentos de variáveis.
/// </summary>
/// <remarks>
/// A classe <c><see cref="ContractGenerator"/></c> utiliza um template em formato DOCX para substituir
/// as variáveis de contrato e gerar um documento final em PDF.
/// </remarks>
public class ContractGenerator
{
    /// <summary>
    /// Gera um contrato a partir de um objeto <c><see cref="Contrato"/></c> 
    /// e de um template opcional <c><see cref="ModeloContrato"/></c>.
    /// </summary>
    /// <remarks>
    /// Este método realiza as seguintes etapas:
    /// <para>1. Carrega o template DOCX do contrato.</para>
    /// <para>2. Substitui as variáveis no documento conforme os
    ///  mapeamentos gerados a partir do objeto <c><see cref="Contrato"/></c>.</para>
    /// <para>3. Salva o documento modificado como DOCX.</para>
    /// <para>4. Converte o arquivo DOCX para PDF utilizando a biblioteca GrapeCity.</para>
    /// <para>5. Exclui o arquivo DOCX temporário.</para>
    /// </remarks>
    /// <param name="contrato">Objeto <c><see cref="Contrato"/></c> 
    /// contendo os dados do contrato a ser gerado.</param>
    /// <param name="model">Objeto opcional do tipo <c><see cref="ModeloContrato"/></c> 
    /// que define o template do contrato.</param>
    public void GenerateContract(Contrato contrato, ModeloContrato? model)
    {
        var pathOutputDocx = Path.Combine(Configuration.ContractsPath, $"{contrato.ArquivoId}.docx");
        var pathTemplateDocx = Path.Combine(Configuration.ContractTemplatesPath, $"{model?.Id}{model?.Extensao}");
        using var doc = DocX.Load(pathTemplateDocx);

        var mappings = new VariablesContractMappings(contrato);
        var fields = mappings.GetFields();

        foreach (var kvp in fields)
        {
            doc.ReplaceText(new StringReplaceTextOptions
            {
                SearchValue = $"%{kvp.Key}%",
                NewValue = kvp.Value,
            });
        }

        doc.SaveAs(pathOutputDocx);

        var wordDoc = new GcWordDocument();
        wordDoc.Load(pathOutputDocx);

        using (var layout = new GcWordLayout(wordDoc))
        {
            var pdfOutputSettings = new PdfOutputSettings
            {
                CompressionLevel = CompressionLevel.Fastest,
                ConformanceLevel = GrapeCity.Documents.Pdf.PdfAConformanceLevel.PdfA1a
            };

            layout.SaveAsPdf(Path.Combine(Configuration.ContractsPath, $"{contrato.ArquivoId}.pdf"), null, pdfOutputSettings);
        }
        File.Delete(pathOutputDocx);
    }
}