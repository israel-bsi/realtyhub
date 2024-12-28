using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface ICustomerHandler
{
    Task<Response<Customer?>> CreateAsync(CreateCustomerRequest request);
    Task<Response<Customer?>> UpdateAsync(UpdateCustomerRequest request);
    Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request);
    Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request);
    Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request);
}