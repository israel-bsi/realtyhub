namespace RealtyHub.Core.Models;

/// <summary>
/// Representa os dados de um arquivo, incluindo seu conteúdo e metadados.
/// </summary>
public class FileData
{
    /// <summary>
    /// Obtém ou define o identificador do arquivo.
    /// </summary>
    /// <value>Uma string representando o ID do arquivo.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o arquivo é uma miniatura.
    /// </summary>
    /// <value><c>true</c> se o arquivo for uma miniatura; caso contrário, <c>false</c>.</value>
    public bool IsThumbnail { get; set; }

    /// <summary>
    /// Obtém ou define o conteúdo binário do arquivo.
    /// </summary>
    /// <value>Um array de bytes representando o conteúdo do arquivo.</value>
    public byte[] Content { get; set; } = [];

    /// <summary>
    /// Obtém ou define o tipo de conteúdo (MIME type) do arquivo.
    /// </summary>
    /// <value>Uma string representando o tipo de conteúdo, por exemplo, "image/png".</value>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome do arquivo.
    /// </summary>
    /// <value>Uma string contendo o nome do arquivo.</value>
    public string Name { get; set; } = string.Empty;
}