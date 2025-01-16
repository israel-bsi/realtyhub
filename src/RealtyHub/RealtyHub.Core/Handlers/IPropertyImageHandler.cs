using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IPropertyImageHandler
{
    Task<Response<PropertyImage?>> CreateAsync(CreatePropertyImageRequest request);
    Task<Response<PropertyImage?>> DeleteAsync();
    Task<Response<PropertyImage?>> GetByIdAsync();
    Task<Response<PropertyImage?>> GetByPropertyAsync();
}