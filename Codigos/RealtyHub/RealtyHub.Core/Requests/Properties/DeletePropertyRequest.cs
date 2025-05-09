namespace RealtyHub.Core.Requests.Properties;

/// <summary>
/// Representa uma requisição para excluir um imóvel.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DeletePropertyRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value> O Id do imóvel a ser excluído. </value>
    public long Id { get; set; }
}