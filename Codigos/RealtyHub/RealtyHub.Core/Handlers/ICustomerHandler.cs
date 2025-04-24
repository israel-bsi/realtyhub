using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface responsável por gerenciar os clientes.
/// </summary>
public interface ICustomerHandler
{
    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="request">O objeto cliente que irá ser criado</param>
    /// <returns></returns>
    Task<Response<Customer?>> CreateAsync(Customer request);
    /// <summary>
    /// Atualiza um cliente existente.
    /// </summary>
    /// <param name="request">O objeto cliente que irá ser atualizado</param>
    /// <returns></returns>
    Task<Response<Customer?>> UpdateAsync(Customer request);
    /// <summary>
    /// Exclui um cliente existente.
    /// </summary>
    /// <param name="request">O objeto request que irá ser deletado</param>
    /// <returns></returns>
    Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request);
    /// <summary>
    /// Obtém um cliente pelo ID.
    /// </summary>
    /// <param name="request">O objeto request que irá ser pesquisado</param>
    /// <returns></returns>
    Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request);
    /// <summary>
    /// Obtém todos os clientes com paginação e filtro.
    /// </summary>
    /// <param name="request">O objeto request com os filtros a serem usados na pesquisa</param>
    /// <returns></returns>
    Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request);
}