using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface ICondominiumHandler
{
    Task<Response<Condominium?>> CreateAsync(Condominium request);
    Task<Response<Condominium?>> UpdateAsync(Condominium request);
    Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request);
    Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request);
    Task<PagedResponse<List<Condominium>?>> GetAllAsync(GetAllCondominiumsRequest request);
}