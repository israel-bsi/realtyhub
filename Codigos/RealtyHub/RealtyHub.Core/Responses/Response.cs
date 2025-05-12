using System.Text.Json.Serialization;

namespace RealtyHub.Core.Responses;

/// <summary>
/// Classe genérica para encapsular a resposta de uma operação.
/// </summary>
/// <typeparam name="TData">Tipo de dados a serem retornados na resposta.</typeparam>
public class Response<TData>
{
    /// <summary>
    /// Código de status HTTP da resposta.
    /// </summary>
    private readonly int _code;

    /// <summary>
    /// Construtor padrão da classe <see cref="Response{TData}"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor inicializa o código de status com o valor padrão definido na configuração.
    /// </remarks>
    [JsonConstructor]
    public Response() => _code = Configuration.DefaultStatusCode;

    /// <summary>
    /// Construtor da classe <see cref="Response{TData}"/> com dados, código de status e mensagem.
    /// </summary>
    /// <remarks>
    /// Este construtor permite inicializar a resposta com dados específicos,
    /// um código de status e uma mensagem personalizada.
    /// </remarks>
    /// <param name="data">Dados a serem retornados na resposta.</param>
    /// <param name="code">Código de status HTTP da resposta.</param>
    /// <param name="message">Mensagem adicional sobre o resultado da operação.</param>

    public Response(TData? data, int code = Configuration.DefaultStatusCode, string? message = null)
    {
        Data = data;
        Message = message;
        _code = code;
    }
    /// <summary>
    /// Dados a serem retornados na resposta.
    /// </summary>
    /// <remarks>
    /// Este campo pode conter qualquer tipo de dado, dependendo do contexto da operação.
    /// </remarks>
    /// <value>Um objeto do tipo <typeparamref name="TData"/> representando os dados da resposta.</value>
    public TData? Data { get; set; }

    /// <summary>
    /// Mensagem adicional sobre o resultado da operação.
    /// </summary>
    /// <remarks>
    /// Esta mensagem pode conter informações sobre erros, avisos ou sucesso da operação.
    /// </remarks>
    /// <value>Uma string contendo a mensagem.</value>
    public string? Message { get; set; }

    /// <summary>
    /// Campo que indica se a operação foi bem-sucedida.
    /// </summary>
    /// <remarks>
    /// Este campo é calculado com base no código de status HTTP.
    /// </remarks>
    /// <value> <c>true</c> se o código de status estiver entre 200 e 299; caso contrário, <c>false</c>.</value>
    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}