namespace RealtyHub.Core.Requests.Offers;

/// <summary>
/// Classe que representa uma reqquisição para rejeitar uma proposta
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class RejectOfferRequest : Request
{
    /// <summary>
    /// Identificador da proposta a ser rejeitada
    /// </summary>
    /// <value>O Id da proposta.</value>
    public long Id { get; set; }
}