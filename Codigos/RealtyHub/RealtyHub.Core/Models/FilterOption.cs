namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma opção de filtro para exibição e mapeamento de propriedades.
/// </summary>
public class FilterOption
{
    /// <summary>
    /// Obtém ou define o nome que será exibido para esta opção de filtro.
    /// </summary>
    /// <value>Uma string contendo o nome a ser exibido.</value>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome da propriedade que será filtrada.
    /// </summary>
    /// <value>Uma string representando o nome da propriedade.</value>
    public string PropertyName { get; set; } = string.Empty;
}