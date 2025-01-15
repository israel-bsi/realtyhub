using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IPropertyImageHandler
{
    Task<Response<PropertyImage?>> CreateAsync();
    Task<Response<PropertyImage?>> DeleteAsync();
    Task<Response<PropertyImage?>> GetByIdAsync();
    Task<Response<PropertyImage?>> GetByPropertyAsync();
}