using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests;

/// <summary>
/// Classe base para todas as solicitações.
/// </summary>
/// <remarks>
/// Esta classe é responsável por encapsular os parâmetros comuns
/// necessários para realizar solicitações,
/// incluindo informações de autenticação e identificação do usuário.
/// </remarks>
public abstract class Request
{
    /// <summary>
    /// Identificador do usuário que está fazendo a solicitação.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para identificar o usuário que está
    /// realizando a solicitação,
    /// permitindo rastrear e auditar as ações realizadas.
    /// </remarks>
    /// <value>O ID do usuário que está fazendo a solicitação.</value>
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}