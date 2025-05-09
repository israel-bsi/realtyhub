namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa uma requisição para obter todas as propostas de um imóvel
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="PagedRequest"/></c>, que contém propriedades comuns para requisições paginadas.
/// </remarks>
public class GetAllOffersByPropertyRequest : PagedRequest
{
    /// <summary>
    /// Identificador do imóvel cujas propostas serão obtidas
    /// </summary>
    /// <value>O Id do imóvel.</value>
    public long PropertyId { get; set; }
}