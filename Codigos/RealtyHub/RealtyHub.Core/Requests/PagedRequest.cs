namespace RealtyHub.Core.Requests;

/// <summary>
/// Classe base para solicitações paginadas.
/// </summary>
/// <remarks>
/// Esta classe é responsável por encapsular os parâmetros comuns
/// necessários para realizar solicitações paginadas,
/// incluindo informações de paginação, termos de pesquisa e filtros.
/// </remarks>
/// <seealso cref="Request" />
public abstract class PagedRequest : Request
{
    /// <summary>
    /// Número da página atual.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar o número da página atual
    /// na solicitação de dados paginados.
    /// O valor padrão é definido na configuração.
    /// </remarks>
    public int PageNumber { get; set; } = Configuration.DefaultPageNumber;

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar o número de itens por página
    /// na solicitação de dados paginados.
    /// O valor padrão é definido na configuração.
    /// </remarks>
    /// <value>O tamanho da página.</value>
    public int PageSize { get; set; } = Configuration.DefaultPageSize;

    /// <summary>
    /// Termo de pesquisa.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar um termo de pesquisa
    /// que pode ser utilizado para filtrar os resultados.
    /// </remarks>
    /// <value>O termo de pesquisa.</value>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Campo de filtro.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar um campo específico
    /// que pode ser utilizado para filtrar os resultados.
    /// </remarks>
    /// <value>O campo de filtro.</value>    
    public string FilterBy { get; set; } = string.Empty;

    /// <summary>
    /// Data de início do filtro.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar uma data de início
    /// que pode ser utilizada para filtrar os resultados.
    /// </remarks>
    /// <value>A data de início do filtro.</value>
    public string? StartDate { get; set; }

    /// <summary>
    /// Data de término do filtro.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para especificar uma data de término
    /// que pode ser utilizada para filtrar os resultados.
    /// </remarks>
    /// <value>A data de término do filtro.</value>
    public string? EndDate { get; set; }
}