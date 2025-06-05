namespace RealtyHub.ApiService;

/// <summary>
/// Representa as configurações globais da aplicação.
/// </summary>
/// <remarks>
/// Esta classe contém configurações importantes, como a política de CORS, 
/// caminhos para diretórios e configurações de e-mail utilizados pelo serviço.
/// </remarks>
public static class Configuration
{
    /// <summary>
    /// Nome da política de CORS utilizada pela aplicação.
    /// </summary>
    public const string CorsPolicyName = "realtyhub";

    /// <summary>
    /// Obtém ou define o caminho para os arquivos de contratos.
    /// </summary>
    public static string ContractsPath { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o caminho para os arquivos de fotos.
    /// </summary>
    public static string PhotosPath { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o caminho para os templates de contrato.
    /// </summary>
    public static string ContractTemplatesPath { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o caminho para os relatórios.
    /// </summary>
    public static string ReportsPath { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o caminho para os logos.
    /// </summary>
    public static string LogosPath { get; set; } = string.Empty;

    /// <summary>
    /// Configurações de e-mail utilizadas pela aplicação.
    /// </summary>
    public static EmailConfiguration EmailSettings { get; } = new();

    /// <summary>
    /// Representa as configurações de e-mail.
    /// </summary>
    public class EmailConfiguration
    {
        /// <summary>
        /// E-mail de origem utilizado para envio das mensagens.
        /// </summary>
        /// <value>O e-mail de origem utilizado para envio das mensagens.</value>
        public const string EmailFrom = "realtyhub.br@gmail.com";

        /// <summary>
        /// Senha do e-mail de origem.
        /// </summary>
        /// <value>A senha do e-mail de origem.</value>
        public string EmailPassword { get; set; } = string.Empty;
    }
}