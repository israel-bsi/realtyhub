using System.Text.Json.Serialization;

namespace RealtyHub.Core.Models;

public class PropertyPhoto : Entity
{
    public string Id { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public bool IsThumbnail { get; set; }
    public long PropertyId { get; set; }
    public bool IsActive { get; set; }
    public string UserId { get; set; } = string.Empty;
    [JsonIgnore]
    public Property Property { get; set; } = null!;

    public override string ToString()
    {
        return $"Id: {Id}, " +
            $"Extension: {Extension}, " +
            $"IsThumbnail: {IsThumbnail}, " +
            $"PropertyId: {PropertyId}, " +
            $"IsActive: {IsActive}, " +
            $"UserId: {UserId}";
    }
}