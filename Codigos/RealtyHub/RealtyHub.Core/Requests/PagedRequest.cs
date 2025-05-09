namespace RealtyHub.Core.Requests;

/// <summary>
/// Classe base para solicitações paginadas.
/// </summary>
/// <remarks>
/// <para>Esta classe é utilizada como base para todas as solicitações paginadas</para>
/// <para>Herda da classe <c><seealso cref="Request"/></c>.</para>
/// </remarks>
public abstract class PagedRequest : Request
{
    /// <summary>
    /// Número da página atual.
    /// </summary>    
    /// <value>O número da página atual.</value>
    public int PageNumber { get; set; } = Configuration.DefaultPageNumber;

    /// <summary>
    /// Tamanho da página.
    /// </summary>    
    /// <value>O tamanho da página.</value>
    public int PageSize { get; set; } = Configuration.DefaultPageSize;

    /// <summary>
    /// Termo de pesquisa.
    /// </summary>    
    /// <value>O termo de pesquisa.</value>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Campo de filtro onde o termo de pesquisa será aplicado.
    /// </summary>    
    /// <value>O campo de filtro.</value>    
    public string FilterBy { get; set; } = string.Empty;

    /// <summary>
    /// Data de início do filtro.
    /// </summary>
    /// <value>A data de início do filtro.</value>
    public string? StartDate { get; set; }

    /// <summary>
    /// Data de término do filtro.
    /// </summary>    
    /// <value>A data de término do filtro.</value>
    public string? EndDate { get; set; }
}