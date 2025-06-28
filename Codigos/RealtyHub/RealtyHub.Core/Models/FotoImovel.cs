using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma foto associada a um imóvel.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Entidade"/></c>, 
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class FotoImovel : Entidade
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
    public string Extensao { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a foto é uma miniatura.
    /// </summary>
    /// <value><c>true</c> se a foto for uma miniatura; caso contrário, <c>false</c>.</value>
    public bool Miniatura { get; set; }

    /// <summary>
    /// Obtém ou define o ID do imóvel associada a esta foto.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da imóvel.</value>
    public long ImovelId { get; set; }

    /// <summary>
    /// Indica se a foto está ativa.
    /// </summary>
    /// <value><c>true</c> se a foto estiver ativa; caso contrário, <c>false</c>.</value>
    public bool Ativo { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado à foto.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o imóvel associada a esta foto.
    /// </summary>
    /// <value>Um objeto <see cref="Imovel"/> representando a imóvel à qual a foto pertence.</value>
    [JsonIgnore]
    public Imovel Imovel { get; set; } = null!;

    /// <summary>
    /// Retorna uma representação em forma de string dos dados da foto.
    /// </summary>
    /// <returns>
    /// Uma string contendo o ID, a extensão, se é miniatura, o ID do imóvel, status ativo e o ID do usuário.
    /// </returns>
    public override string ToString()
    {
        return $"Id: {Id}, " +
            $"Extension: {Extensao}, " +
            $"IsThumbnail: {Miniatura}, " +
            $"PropertyId: {ImovelId}, " +
            $"IsActive: {Ativo}, " +
            $"UserId: {UsuarioId}";
    }
}