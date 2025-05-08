namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma entidade base com propriedades de data de criação e atualização.
/// </summary>
/// <remarks>
/// Esta classe é utilizada como base para outras entidades no sistema,
/// fornecendo um padrão para o gerenciamento de datas de criação e atualização.
/// </remarks>
public class Entity
{

    /// <summary>
    /// Obtém ou define a data de criação da entidade.
    /// </summary>
    /// <value>
    /// Uma <c><see cref="DateTime"/></c> representando a data e hora em que a entidade foi criada.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Obtém ou define a data de atualização da entidade.
    /// </summary>
    /// <value>
    /// Uma <c><see cref="DateTime"/></c> representando a data e hora da última atualização da entidade.
    /// </value>
    public DateTime UpdatedAt { get; set; }
}