namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa um pedido para aceitar uma oferta
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class AcceptOfferRequest : Request
{
    /// <summary>
    /// Identificador da oferta a ser aceita
    /// </summary>
    /// <value>O Id da oferta a ser aceita.</value>
    public long Id { get; set; }
}