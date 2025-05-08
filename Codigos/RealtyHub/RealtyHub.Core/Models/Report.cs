namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um relatório no sistema.
/// </summary>
public class Report
{
    /// <summary>
    /// Obtém ou define a URL do relatório.
    /// </summary>
    /// <value>Uma string representando a URL onde o relatório pode ser acessado.</value>
    public string Url { get; set; } = string.Empty;
}