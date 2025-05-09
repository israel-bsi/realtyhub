using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests;

/// <summary>
/// Classe base para todas as requisições.
/// </summary>
public abstract class Request
{
    /// <summary>
    /// Identificador do usuário que está fazendo a requisição.
    /// </summary>    
    /// <remarks>    
    /// Essa propriedade é usada para fins de auditoria e controle de acesso.
    /// </remarks>
    /// <value>O ID do usuário que está fazendo a requisição.</value>
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}