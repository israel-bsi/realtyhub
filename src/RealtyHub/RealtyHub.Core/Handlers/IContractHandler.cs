using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IContractHandler
{
    Task<Response<Contract?>> CreateAsync(CreateContractRequest request);
    Task<Response<Contract?>> UpdateAsync(UpdateContractRequest request);
    Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request);
    Task<Response<Contract?>> GetByIdAsync(GetContractByIdRequest request);
    Task<PagedResponse<List<Contract>?>> GetAllAsync(GetAllContractsRequest request);
}