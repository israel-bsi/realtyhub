namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um item de foto associado a uma propriedade.
/// </summary>
public class PhotoItem
{
    /// <summary>
    /// Obtém ou define o identificador da foto.
    /// </summary>
    /// <value>Uma string representando o ID da foto.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o identificador da propriedade associada a esta foto.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da propriedade.</value>
    public long PropertyId { get; set; }

    /// <summary>
    /// Indica se a foto é uma miniatura.
    /// </summary>
    /// <value><c>true</c> se for uma miniatura; caso contrário, <c>false</c>.</value>
    public bool IsThumbnail { get; set; }

    /// <summary>
    /// Obtém ou define a URL de exibição da foto.
    /// </summary>
    /// <value>Uma string representando a URL para exibição da foto.</value>
    public string DisplayUrl { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o conteúdo binário da foto.
    /// </summary>
    /// <value>Um array de bytes representando o conteúdo da foto. Pode ser nulo.</value>
    public byte[]? Content { get; set; } = [];

    /// <summary>
    /// Obtém ou define o tipo de conteúdo (MIME type) da foto.
    /// </summary>
    /// <value>Uma string representando o tipo de conteúdo, por exemplo, "image/jpeg". Pode ser nulo.</value>
    public string? ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome original do arquivo da foto.
    /// </summary>
    /// <value>Uma string contendo o nome original do arquivo. Pode ser nulo.</value>
    public string? OriginalFileName { get; set; } = string.Empty;
}