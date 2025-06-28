using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de clientes.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com os clientes, como criação, atualização, exclusão e recuperação de informações.
/// </remarks>
public interface ICustomerHandler
{
    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar um novo cliente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Cliente"/> contendo as informações do novo cliente.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Cliente"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Cliente?>> CreateAsync(Cliente request);

    /// <summary>
    /// Atualiza um cliente existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de um cliente existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Cliente"/> contendo as informações atualizadas do cliente.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Cliente"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Cliente?>> UpdateAsync(Cliente request);

    /// <summary>
    /// Exclui um cliente existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por excluir um cliente existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DeleteCustomerRequest"/> contendo o Id do cliente a ser excluído.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Cliente"/> e pode ser nulo se a operação falhar.
    /// </returns>    
    Task<Response<Cliente?>> DeleteAsync(DeleteCustomerRequest request);

    /// <summary>
    /// Obtém um cliente pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar as informações de um cliente existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetCustomerByIdRequest"/> contendo o Id do cliente a ser recuperado.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Cliente"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Cliente?>> GetByIdAsync(GetCustomerByIdRequest request);

    /// <summary>
    /// Obtém todos os clientes com paginação e filtro.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar todos os clientes existentes com base nos filtros fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllCustomersRequest"/> contendo os filtros e paginação.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Cliente"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Cliente>?>> GetAllAsync(GetAllCustomersRequest request);
}