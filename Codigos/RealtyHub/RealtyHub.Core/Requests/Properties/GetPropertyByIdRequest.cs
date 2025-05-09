namespace RealtyHub.Core.Requests.Properties;

/// <summary>
/// Classe que representa uma requisição para obter todas as visitas de um imóvel
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetPropertyByIdRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value> O Id do imóvel a ser obtido. </value>
    public long Id { get; set; }
}