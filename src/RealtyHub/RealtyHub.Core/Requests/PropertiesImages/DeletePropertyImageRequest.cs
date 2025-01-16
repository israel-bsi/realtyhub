namespace RealtyHub.Core.Requests.PropertiesImages;

public class DeletePropertyImageRequest : Request
{
    public string ImageId { get; set; } = string.Empty;
    public long PropertyId { get; set; }
}