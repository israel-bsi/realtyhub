namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa uma requisição para obter uma proposta específica
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetOfferByIdRequest : Request
{
    /// <summary>
    /// Identificador da proposta a ser obtida
    /// </summary>
    /// <value>O Id da proposta.</value>
    public long Id { get; set; }
}