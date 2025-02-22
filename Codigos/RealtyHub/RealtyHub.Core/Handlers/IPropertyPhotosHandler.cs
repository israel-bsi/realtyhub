using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IPropertyPhotosHandler
{
    Task<Response<PropertyPhoto?>> CreateAsync(CreatePropertyPhotosRequest request);
    Task<Response<List<PropertyPhoto>?>> UpdateAsync(UpdatePorpertyPhotosRequest request);
    Task<Response<PropertyPhoto?>> DeleteAsync(DeletePropertyPhotoRequest request);
    Task<Response<List<PropertyPhoto>?>> GetAllByPropertyAsync(
        GetAllPropertyPhotosByPropertyRequest request);
}