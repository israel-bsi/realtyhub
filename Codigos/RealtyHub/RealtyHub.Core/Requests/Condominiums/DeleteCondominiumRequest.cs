namespace RealtyHub.Core.Requests.Condominiums;

/// <summary>
/// Classe que representa uma requisição para excluir um condomínio.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DeleteCondominiumRequest : Request
{
    /// <summary>
    /// Identificador do condomínio a ser excluído.
    /// </summary>
    /// <value>O Id do condomínio a ser excluído.</value>
    public long Id { get; set; }
}