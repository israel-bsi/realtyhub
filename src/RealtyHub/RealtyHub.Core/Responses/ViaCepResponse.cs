using System.Text.Json.Serialization;

namespace RealtyHub.Core.Responses;

public class ViaCepResponse
{
    [JsonPropertyName("cep")]
    public string ZipCode { get; set; } = string.Empty;
    [JsonPropertyName("logradouro")]
    public string Street { get; set; } = string.Empty;
    [JsonPropertyName("complemento")]
    public string Complement { get; set; } = string.Empty;
    [JsonPropertyName("bairro")]
    public string Neighborhood { get; set; } = string.Empty;
    [JsonPropertyName("localidade")]
    public string City { get; set; } = string.Empty;
    [JsonPropertyName("estado")]
    public string State { get; set; } = string.Empty;
}