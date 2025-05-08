namespace RealtyHub.Core.Models.Account;

/// <summary>
/// Representa a requisição para confirmar o e-mail do usuário.
/// </summary>
public class ConfirmEmailRequest
{
    /// <summary>
    /// ID do usuário que está solicitando a confirmação de e-mail.
    /// </summary>
    /// <value>O ID do usuário.</value>
    public long UserId { get; set; }

    /// <summary>
    /// Token de confirmação de e-mail.
    /// </summary>
    /// <value>O token de confirmação.</value>
    public string Token { get; set; } = string.Empty;
}