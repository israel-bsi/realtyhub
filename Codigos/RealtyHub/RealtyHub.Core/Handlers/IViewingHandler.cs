using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IViewingHandler
{
    Task<Response<Viewing?>> ScheduleAsync(Viewing request);
    Task<Response<Viewing?>> RescheduleAsync(Viewing request);
    Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request);
    Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request);
    Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request);
    Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request);
}