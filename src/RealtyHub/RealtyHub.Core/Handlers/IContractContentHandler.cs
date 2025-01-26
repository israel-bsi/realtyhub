using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IContractContentHandler
{
    Task<Response<ContractContent?>> CreateAsync(CreateContractContentRequest request);
    Task<Response<ContractContent?>> UpdateAsync(UpdateContractContentRequest request);
    Task<Response<ContractContent?>> DeleteAsync(DeleteContractContentRequest request);
    Task<Response<List<ContractContent>?>> GetAllByUserAsync(GetAllContractContentByUserRequest request);
}