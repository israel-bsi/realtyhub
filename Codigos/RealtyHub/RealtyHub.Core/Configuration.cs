namespace RealtyHub.Core;

/// <summary>
/// Representa as configurações da aplicação.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Obtém o código de status padrão para as respostas.
    /// </summary>
    /// <value>Um inteiro representando o código de status padrão.</value>
    public const int DefaultStatusCode = 200;

    /// <summary>
    /// Obtém o número da página padrão para paginação.
    /// </summary>
    /// <value>Um inteiro representando o número da página padrão.</value>
    public const int DefaultPageNumber = 1;

    /// <summary>
    /// Obtém o locale utilizado pela aplicação.
    /// </summary>
    /// <value>Uma string representando o locale, por exemplo, "pt_BR".</value>
    public const string Locale = "pt_BR";

    /// <summary>
    /// Obtém o tamanho padrão da página para paginação.
    /// </summary>
    /// <value>Um inteiro representando o tamanho da página padrão.</value>
    public const int DefaultPageSize = 25;

    /// <summary>
    /// Obtém ou define a string de conexão utilizada para acessar o banco de dados.
    /// </summary>
    /// <value>Uma string representando a string de conexão.</value>
    public static string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a URL do backend utilizada para chamadas à API.
    /// </summary>
    /// <value>Uma string representando a URL do backend.</value>
    public static string BackendUrl { get; set; } = "http://localhost:5538";

    /// <summary>
    /// Obtém ou define a URL do frontend utilizada para a interface web.
    /// </summary>
    /// <value>Uma string representando a URL do frontend.</value>
    public static string FrontendUrl { get; set; } = "http://localhost:5187";
}