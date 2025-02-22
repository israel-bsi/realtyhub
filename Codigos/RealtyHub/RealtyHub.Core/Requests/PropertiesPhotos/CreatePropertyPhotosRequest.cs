using Microsoft.AspNetCore.Http;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.PropertiesPhotos;

public class CreatePropertyPhotosRequest : Request
{
    public long PropertyId { get; set; }
    public HttpRequest? HttpRequest { get; set; }
    public List<FileData>? FileBytes { get; set; }
}