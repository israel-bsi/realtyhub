using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class CustomerHandler : ICustomerHandler
{
    public Task<Response<Customer?>> CreateAsync(CreateCustomerRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Customer?>> UpdateAsync(UpdateCustomerRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        throw new NotImplementedException();
    }
}