using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests.Account;

/// <summary>
/// Classe que representa uma requisição de registro de usuário.
/// </summary>
/// <remarks>
/// <para>Esta classe contém as propriedades necessárias para o registro de um novo usuário,
/// incluindo o número do CRECI, nome, e-mail e senha.</para>
/// <para>As propriedades são validadas para garantir que os dados 
/// fornecidos estejam corretos e atendam aos requisitos mínimos.</para>
/// </remarks>
public class RegisterRequest : Request
{
    /// <summary>
    /// Número do CRECI do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um valor válido.</para>
    /// <para>O número do CRECI é um registro profissional necessário para corretores de imóveis.</para>
    /// </remarks>
    /// <value>O número do CRECI do usuário.</value>
    [Required(ErrorMessage = "Creci é obrigatório")]
    public string Creci { get; set; } = string.Empty;

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    /// <remarks>
    /// Esta propriedade é obrigatória e deve ser preenchida com o nome completo do usuário.
    /// </remarks>
    /// <value>O nome completo do usuário.</value>
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com um endereço de e-mail válido.</para>
    /// <para>O e-mail é utilizado para comunicação e recuperação de senha.</para>
    /// </remarks>
    /// <value>O endereço de e-mail do usuário.</value>
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com uma senha forte.</para>
    /// <para>A senha deve ter no mínimo 6 caracteres.</para>
    /// </remarks>
    /// <value>A senha do usuário.</value>
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha do usuário.
    /// </summary>
    /// <remarks>
    /// <para>Esta propriedade é obrigatória e deve ser preenchida com a mesma senha fornecida anteriormente.</para>
    /// <para>A confirmação de senha é utilizada para garantir que o usuário não cometeu erros de digitação ao inserir a senha.</para>
    /// </remarks> 
    /// <value>A confirmação da senha do usuário.</value>
    [Required(ErrorMessage = "Senha inválida")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;
}