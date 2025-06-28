using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de contratos.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com os contratos, como criação, atualização, exclusão e recuperação de informações.
/// </remarks>
public interface IContractHandler
{
    /// <summary>
    /// Cria um novo contrato.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar um novo contrato com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Contrato"/> contendo as informações do novo contrato.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Contrato"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Contrato?>> CreateAsync(Contrato request);

    /// <summary>
    /// Atualiza as informações de um contrato existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de um contrato existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Contrato"/> contendo as informações atualizadas do contrato.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Contrato"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Contrato?>> UpdateAsync(Contrato request);

    /// <summary>
    /// Exclui um contrato existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por excluir um contrato existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DeleteContractRequest"/> contendo o Id do contrato a ser excluído.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Contrato"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Contrato?>> DeleteAsync(DeleteContractRequest request);

    /// <summary>
    /// Recupera as informações de um contrato existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar as informações de um contrato existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetContractByIdRequest"/> contendo o Id do contrato a ser recuperado.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Contrato"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Contrato?>> GetByIdAsync(GetContractByIdRequest request);

    /// <summary>
    /// Recupera uma lista de contratos com base nos critérios de pesquisa fornecidos.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar uma lista de contratos com base nos critérios de pesquisa fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllContractsRequest"/> contendo os critérios de pesquisa.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Contrato"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Contrato>?>> GetAllAsync(GetAllContractsRequest request);
}