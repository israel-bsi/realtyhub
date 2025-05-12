using System.Text.Json.Serialization;

namespace RealtyHub.Core.Responses;

/// <summary>
/// Classe genérica para encapsular a resposta de uma operação paginada.
/// </summary>
/// <typeparam name="TData">Tipo de dados a serem retornados na resposta.</typeparam>
public class PagedResponse<TData> : Response<TData>
{
    /// <summary>
    /// Construtor padrão da classe <see cref="PagedResponse{TData}"/>.
    /// </summary>
    /// <param name="data">Dados a serem retornados na resposta.</param>
    /// <param name="totalCount">Total de itens disponíveis.</param>
    /// <param name="currentPage">Número da página atual.</param>
    /// <param name="pageSize">Tamanho da página.</param>
    /// <remarks>
    /// Este construtor inicializa a resposta paginada com os dados, total de itens,
    /// número da página atual e tamanho da página.
    /// <para> O valor padrão de <c><paramref name="currentPage"/></c> e de <c><paramref name="pageSize"/></c> é 
    /// definido na classe <c><see cref="Configuration"/></c>.</para>
    /// </remarks>
    [JsonConstructor]
    public PagedResponse(TData? data, int totalCount,
        int currentPage = Configuration.DefaultPageNumber,
        int pageSize = Configuration.DefaultPageSize) : base(data)
    {
        Data = data;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    /// <summary>
    /// Construtor da classe <see cref="PagedResponse{TData}"/> com dados, código de status e mensagem.
    /// </summary>
    /// <remarks>
    /// Este construtor permite inicializar a resposta com dados específicos,
    /// um código de status e uma mensagem personalizada.
    /// </remarks>
    /// <param name="data">Dados a serem retornados na resposta.</param>
    /// <param name="code">Código de status HTTP da resposta.</param>
    /// <param name="message">Mensagem adicional sobre o resultado da operação.</param>
    public PagedResponse(TData? data, int code = Configuration.DefaultStatusCode,
        string? message = null) : base(data, code, message) { }

    /// <summary>
    /// Número da página atual.
    /// </summary>
    /// <value>Valor em inteiro da página atual</value>    
    public int CurrentPage { get; set; }

    /// <summary>
    /// Obtém ou define o número total de páginas disponíveis.
    /// </summary>
    /// <remarks> O campo é calculado dividindo 
    /// o <c><see cref="TotalCount"/></c> por <c><see cref="PageSize"/></c> .
    /// </remarks>
    /// <value>Valor em inteiro do número total de páginas</value>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Obtém ou define o tamanho da página.
    /// </summary>
    /// <remarks> O valor padrão é 25.</remarks>
    /// <value>Valor em inteiro do tamanho da página</value>
    public int PageSize { get; set; } = Configuration.DefaultPageSize;

    /// <summary>
    /// Obtém ou define o número total de itens disponíveis.
    /// </summary>
    /// <value>Valor em inteiro do número total de itens</value>
    public int TotalCount { get; set; }
}