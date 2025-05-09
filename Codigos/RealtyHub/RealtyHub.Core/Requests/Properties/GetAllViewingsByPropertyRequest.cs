namespace RealtyHub.Core.Requests.Properties;

/// <summary>
/// Representa uma requisição para obter todas as visitas de um imóvel.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="PagedRequest"/></c>, que contém propriedades comuns para requisições paginadas.
/// </remarks>
public class GetAllViewingsByPropertyRequest : PagedRequest
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value> O Id do imóvel cujas visitas serão obtidas. </value>
    public long PropertyId { get; set; }
}