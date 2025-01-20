namespace RealtyHub.Core.Models;

public class PhotoItem
{
    public string Id { get; set; } = string.Empty;
    public long PropertyId { get; set; }
    public bool IsThumbnail { get; set; }
    public string DisplayUrl { get; set; } = string.Empty;
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? OriginalFileName { get; set; }
}