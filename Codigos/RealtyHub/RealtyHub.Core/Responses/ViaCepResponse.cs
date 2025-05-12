using System.Text.Json.Serialization;

namespace RealtyHub.Core.Responses;

/// <summary>
/// Representa a resposta da API ViaCep contendo informações do endereço.
/// </summary>
public class ViaCepResponse
{
    /// <summary>
    /// Obtém ou define o logradouro.
    /// </summary>
    /// <value>Uma string contendo o logradouro.</value>
    [JsonPropertyName("logradouro")]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o complemento do endereço.
    /// </summary>
    /// <value>Uma string contendo o complemento, se houver.</value>
    [JsonPropertyName("complemento")]
    public string Complement { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o bairro.
    /// </summary>
    /// <value>Uma string contendo o bairro.</value>
    [JsonPropertyName("bairro")]
    public string Neighborhood { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a cidade.
    /// </summary>
    /// <value>Uma string contendo a cidade.</value>
    [JsonPropertyName("localidade")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o estado.
    /// </summary>
    /// <value>Uma string contendo o estado.</value>
    [JsonPropertyName("estado")]
    public string State { get; set; } = string.Empty;
}