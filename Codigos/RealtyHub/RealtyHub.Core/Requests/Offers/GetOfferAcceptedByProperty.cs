namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa uma requisição para obter todas as propostas aceitas de um imóvel
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetOfferAcceptedByProperty : Request
{
    /// <summary>
    /// Identificador do imóvel cujas propostas aceitas serão obtidas
    /// </summary>
    /// <value>O Id do imóvel.</value>
    public long PropertyId { get; set; }
}