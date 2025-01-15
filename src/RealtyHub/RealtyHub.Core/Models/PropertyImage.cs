using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

public class PropertyImage : Entity
{
    public string Id { get; set; } = string.Empty;
    public long PropertyId { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
    [JsonIgnore]
    public Property Property { get; set; } = new();
}