using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesPhotos;

public class UpdatePorpertyPhotosRequest : Request
{
    public long PropertyId { get; set; }
    public List<PropertyPhoto> Photos { get; set; } = [];

    public override string ToString()
    {
        var property = $"PropertyId: {PropertyId}";
        var photos = string.Join(", ", Photos.Select(p => p.ToString()));
        return string.Join(", ", photos, property);
    }
}