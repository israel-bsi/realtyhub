using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma foto associada a um imóvel.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Entity"/></c>, 
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class PropertyPhoto : Entity
{
    /// <summary>
    /// Obtém ou define o identificador único da foto.
    /// </summary>
    /// <value>Uma string representando o ID da foto.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a extensão do arquivo da foto.
    /// </summary>
    /// <value>Uma string representando a extensão, por exemplo, ".jpg" ou ".png".</value>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a foto é uma miniatura.
    /// </summary>
    /// <value><c>true</c> se a foto for uma miniatura; caso contrário, <c>false</c>.</value>
    public bool IsThumbnail { get; set; }

    /// <summary>
    /// Obtém ou define o ID do imóvel associada a esta foto.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da imóvel.</value>
    public long PropertyId { get; set; }

    /// <summary>
    /// Indica se a foto está ativa.
    /// </summary>
    /// <value><c>true</c> se a foto estiver ativa; caso contrário, <c>false</c>.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado à foto.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o imóvel associada a esta foto.
    /// </summary>
    /// <value>Um objeto <see cref="Property"/> representando a imóvel à qual a foto pertence.</value>
    [JsonIgnore]
    public Property Property { get; set; } = null!;

    /// <summary>
    /// Retorna uma representação em forma de string dos dados da foto.
    /// </summary>
    /// <returns>
    /// Uma string contendo o ID, a extensão, se é miniatura, o ID do imóvel, status ativo e o ID do usuário.
    /// </returns>
    public override string ToString()
    {
        return $"Id: {Id}, " +
            $"Extension: {Extension}, " +
            $"IsThumbnail: {IsThumbnail}, " +
            $"PropertyId: {PropertyId}, " +
            $"IsActive: {IsActive}, " +
            $"UserId: {UserId}";
    }
}