using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Payments;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IPaymentHandler
{
    Task<Response<Payment?>> CreateAsync(CreatePaymentRequest request);
    Task<Response<Payment?>> UpdateAsync(UpdatePaymentRequest request);
    Task<Response<Payment?>> DeleteAsync(DeletePaymentRequest request);
    Task<Response<Payment?>> GetByIdAsync(GetPaymentByIdRequest request);
    Task<Response<List<Payment>?>> GetAllAsync(GetAllPaymentsRequest request);
}