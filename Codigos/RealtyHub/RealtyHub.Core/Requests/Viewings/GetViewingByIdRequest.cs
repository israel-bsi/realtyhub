namespace RealtyHub.Core.Requests.Viewings;

/// <summary>
/// Representa uma requisição para obter uma visita a partir do seu identificador.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetViewingByIdRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único da visita.
    /// </summary>
    /// <value>O identificador da visita que será retornada.</value>
    public long Id { get; set; }
}