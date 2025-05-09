namespace RealtyHub.Core.Requests.PropertiesPhotos;

/// <summary>
/// Representa uma requisição para obter todas as fotos de um imóvel.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class GetAllPropertyPhotosByPropertyRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value>O identificador do imóvel cujas fotos serão retornadas.</value>
    public long PropertyId { get; set; }
}