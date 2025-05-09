namespace RealtyHub.Core.Requests.Viewings;

/// <summary>
/// Representa uma requisição para marcar uma visita como concluída.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DoneViewingRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único da visita.
    /// </summary>
    /// <value>O identificador da visita a ser marcada como concluída.</value>
    public long Id { get; set; }
}