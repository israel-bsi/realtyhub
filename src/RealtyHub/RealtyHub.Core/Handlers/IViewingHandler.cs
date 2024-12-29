using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IViewingHandler
{
    Task<Response<Viewing?>> CreateAsync(CreateViewingRequest request);
    Task<Response<Viewing?>> UpdateAsync(UpdateViewingRequest request);
    Task<Response<Viewing?>> DeleteAsync(DeleteViewingRequest request);
    Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request);
    Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request);
}