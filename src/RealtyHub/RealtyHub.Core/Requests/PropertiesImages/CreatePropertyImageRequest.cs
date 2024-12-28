namespace RealtyHub.Core.Requests.PropertiesImages;

public class CreatePropertyImageRequest : Request
{
    public string FilePath { get; set; } = string.Empty;
    public long PropertyId { get; set; }
}