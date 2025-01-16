using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

public class PropertyPhoto : Entity
{
    public string Id { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public long PropertyId { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
    [JsonIgnore]
    public Property Property { get; set; } = new();
}