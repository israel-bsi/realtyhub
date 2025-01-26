using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.ContractsContent;
using RealtyHub.Core.Responses;

namespace RealtyHub.Web.Handlers;

public class ContractContentHandler : IContractContentHandler
{
    public Task<Response<ContractContent?>> CreateAsync(CreateContractContentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<ContractContent?>> UpdateAsync(UpdateContractContentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<ContractContent?>> DeleteAsync(DeleteContractContentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<ContractContent>?>> GetAllByUserAsync(GetAllContractContentByUserRequest request)
    {
        throw new NotImplementedException();
    }
}