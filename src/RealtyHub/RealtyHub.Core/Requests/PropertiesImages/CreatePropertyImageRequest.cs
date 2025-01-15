using Microsoft.AspNetCore.Http;

namespace RealtyHub.Core.Requests.PropertiesImages;

public class CreatePropertyImageRequest : Request
{
    public long PropertyId { get; set; }
    public HttpRequest HttpRequest { get; set; } = null!;
}