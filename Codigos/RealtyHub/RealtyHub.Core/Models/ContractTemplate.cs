using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um template de contrato no sistema.
/// </summary>
public class ContractTemplate
{
    /// <summary>
    /// Obtém ou define o identificador do template.
    /// </summary>
    /// <value>Uma string representando o ID do template.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a extensão do arquivo do template.
    /// </summary>
    /// <value>Uma string representando a extensão do arquivo (por exemplo, ".pdf").</value>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome do template.
    /// </summary>
    /// <value>Uma string contendo o nome do template.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o tipo do modelo de contrato.
    /// </summary>
    /// <value>Um valor do enum <see cref="EContractModelType"/> representando o tipo do modelo.</value>
    public EContractModelType Type { get; set; }

    /// <summary>
    /// Indica se o template deve ser exibido na página.
    /// </summary>
    /// <value><c>true</c> se o template deve ser exibido; caso contrário, <c>false</c>.</value>
    public bool ShowInPage { get; set; }

    /// <summary>
    /// Obtém o caminho completo para o arquivo do template de contrato.
    /// </summary>
    /// <value>
    /// Uma string representando a URL completa construída a partir do backend.
    /// </value>
    public string Path =>
        $"{Configuration.BackendUrl}/contracts-templates/{Id}{Extension}";
}