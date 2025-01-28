using RealtyHub.Core.Models;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace RealtyHub.ApiService.Services;

public class DocXContractGenerator
{
    public void GenerateContractDocx(string templatePath, string outputPath, Contract contract)
    {
        using var doc = DocX.Load(templatePath);

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

        doc.SaveAs(outputPath);
    }
}