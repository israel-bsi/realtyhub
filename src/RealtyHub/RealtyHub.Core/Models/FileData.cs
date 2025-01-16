namespace RealtyHub.Core.Models;

public class FileData
{
    public byte[] Content { get; set; } = null!;
    public string ContentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}