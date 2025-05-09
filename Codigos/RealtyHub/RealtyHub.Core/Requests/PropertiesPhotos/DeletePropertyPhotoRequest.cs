namespace RealtyHub.Core.Requests.PropertiesPhotos;

/// <summary>
/// Representa uma requisição para excluir uma foto de um imóvel.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class DeletePropertyPhotoRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único da foto.
    /// </summary>
    /// <value>Uma string representando o identificador da foto a ser excluída.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o identificador único do imóvel associado à foto.
    /// </summary>
    /// <value>O identificador do imóvel ao qual a foto pertence.</value>
    public long PropertyId { get; set; }
}