using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IContractTemplateHandler
{
    Task<Response<List<ContractTemplate>?>> GetAllAsync();
}