using System.Text.Json.Serialization;

namespace RealtyHub.Core.Responses;

/// <summary>
/// Classe genérica para encapsular a resposta de uma operação, incluindo dados, mensagem e código de status.
/// </summary>
/// <remarks>
/// <para>Esta classe é utilizada em conjunto com os manipuladores de operações para retornar
/// informações sobre o resultado da operação realizada, seja ela uma criação, atualização,
/// recuperação ou exclusão de dados.</para>
/// <para>O código de status HTTP é utilizado para indicar o sucesso ou falha da operação,
/// e a mensagem pode conter informações adicionais sobre o resultado.</para>
/// </remarks>
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
    public TData? Data { get; set; }
    /// <summary>
    /// Mensagem adicional sobre o resultado da operação.
    /// </summary>
    /// <remarks>
    /// Esta mensagem pode conter informações sobre erros, avisos ou sucesso da operação.
    /// </remarks>
    public string? Message { get; set; }
    /// <summary>
    /// Campo que indica se a operação foi bem-sucedida.
    /// </summary>
    /// <remarks>
    /// Este campo é calculado com base no código de status HTTP.
    /// </remarks>
    [JsonIgnore]
    public bool IsSuccess => _code is >= 200 and <= 299;
}