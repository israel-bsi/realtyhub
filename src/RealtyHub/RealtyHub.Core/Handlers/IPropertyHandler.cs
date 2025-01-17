using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IPropertyHandler
{
    Task<Response<Property?>> CreateAsync(Property request);
    Task<Response<Property?>> UpdateAsync(Property request);
    Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request);
    Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request);
    Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request);
    Task<Response<List<Viewing>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request);
}