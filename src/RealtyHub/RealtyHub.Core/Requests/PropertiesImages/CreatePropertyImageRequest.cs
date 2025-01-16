using Microsoft.AspNetCore.Http;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesImages;

public class CreatePropertyImageRequest : Request
{
    public long PropertyId { get; set; }
    public HttpRequest? HttpRequest { get; set; }
    public List<FileData>? FileBytes { get; set; }
}