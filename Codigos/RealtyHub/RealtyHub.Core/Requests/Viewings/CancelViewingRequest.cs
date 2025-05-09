namespace RealtyHub.Core.Requests.Viewings;

/// <summary>
/// Representa uma requisição para cancelar uma visita.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class CancelViewingRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único da visita.
    /// </summary>
    /// <value>O identificador da visita a ser cancelada.</value>
    public long Id { get; set; }
}