namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa uma requisição para obter todas as propostas de um cliente
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="PagedRequest"/></c>, que contém propriedades comuns para requisições paginadas.
/// </remarks>
public class GetAllOffersByCustomerRequest : PagedRequest
{
    /// <summary>
    /// Identificador do cliente cujas propostas serão obtidas
    /// </summary>
    /// <value>O Id do cliente.</value>
    public long CustomerId { get; set; }
}