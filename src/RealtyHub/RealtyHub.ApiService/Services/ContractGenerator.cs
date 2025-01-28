using System.IO.Compression;
using GrapeCity.Documents.Word;
using GrapeCity.Documents.Word.Layout;
using RealtyHub.Core.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace RealtyHub.ApiService.Services;

public class ContractGenerator
{
    public void GenerateContract(string pathContracts, Contract contract, ContractContent? model)
    {

        var pathOutputDocx = Path.Combine(pathContracts, $"{contract.FileId}.docx");
        var pathTemplateDocx = Path.Combine(pathContracts, $"{model?.Id}{model?.Extension}");
        using var doc = DocX.Load(pathTemplateDocx);

        var mappings = new VariablesContractMappings(contract);
        var fiels = mappings.GetFields();

        foreach (var kvp in fiels)
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

        using (var layoyt = new GcWordLayout(wordDoc))
        {
            var pdfOutpuSettings = new PdfOutputSettings
            {
                CompressionLevel = CompressionLevel.Fastest,
                ConformanceLevel = GrapeCity.Documents.Pdf.PdfAConformanceLevel.PdfA1a
            };

            layoyt.SaveAsPdf(Path.Combine(pathContracts, $"{contract.FileId}.pdf"), null, pdfOutpuSettings);
        }
        File.Delete(pathOutputDocx);
    }
}