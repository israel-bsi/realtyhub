namespace RealtyHub.Core.Requests.PropertiesPhotos;

public class DeletePropertyPhotoRequest : Request
{
    public string Id { get; set; } = string.Empty;
    public long PropertyId { get; set; }
}