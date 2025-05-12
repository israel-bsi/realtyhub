namespace RealtyHub.Core.Responses;

/// <summary>
/// Representa a resposta do usuário no sistema.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Obtém ou define o nome do usuário.
    /// </summary>
    /// <value>Uma string contendo o nome do usuário.</value>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o CRECI do usuário.
    /// </summary>
    /// <value>Uma string representando o CRECI do usuário.</value>
    public string Creci { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o e-mail do usuário.
    /// </summary>
    /// <value>Uma string contendo o e-mail do usuário.</value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o nome de usuário.
    /// </summary>
    /// <value>Uma string contendo o nome de usuário.</value>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define as claims associadas ao usuário.
    /// </summary>
    /// <value>Um dicionário que mapeia os nomes das claims aos seus respectivos valores.</value>
    public Dictionary<string, string> Claims { get; set; } = new();
}