using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesPhotos;

public class UpdatePorpertyPhotosRequest : Request
{
    public long PropertyId { get; set; }
    public List<PropertyPhoto> Photos { get; set; } = new();
}