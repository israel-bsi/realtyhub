using System.Text.Json.Serialization;

namespace RealtyHub.Core.Requests;

public abstract class Request
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}