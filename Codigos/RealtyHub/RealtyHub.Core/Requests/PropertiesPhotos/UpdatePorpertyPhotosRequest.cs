using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesPhotos;

/// <summary>
/// Representa uma requisição para atualizar as fotos de um imóvel.
/// </summary>
/// <remarks>
/// A classe herda de <c><see cref="Request"/></c>, que contém propriedades comuns para requisições.
/// </remarks>
public class UpdatePropertyPhotosRequest : Request
{
    /// <summary>
    /// Obtém ou define o identificador único do imóvel.
    /// </summary>
    /// <value>O identificador do imóvel cujas fotos serão atualizadas.</value>
    public long PropertyId { get; set; }

    /// <summary>
    /// Obtém ou define a lista de fotos do imóvel.
    /// </summary>
    /// <value>
    /// Uma lista de <c><see cref="PropertyPhoto"/></c> representando as fotos que serão atualizadas.
    /// </value>
    public List<PropertyPhoto> Photos { get; set; } = [];

    /// <summary>
    /// Retorna uma representação em string dos detalhes da requisição.
    /// </summary>
    /// <returns>
    /// Uma string contendo o identificador do imóvel e os detalhes das fotos.
    /// </returns>
    public override string ToString()
    {
        var property = $"PropertyId: {PropertyId}";
        var photos = string.Join(", ", Photos.Select(p => p.ToString()));
        return string.Join(", ", photos, property);
    }
}