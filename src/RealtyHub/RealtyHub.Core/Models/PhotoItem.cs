namespace RealtyHub.Core.Models;

public class PhotoItem
{
    public string Id { get; set; } = string.Empty;
    public string DisplayUrl { get; set; } = string.Empty;
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? OriginalFileName { get; set; }
    public bool IsNew => string.IsNullOrEmpty(Id);
    public bool MarkedForRemoval { get; set; }
}