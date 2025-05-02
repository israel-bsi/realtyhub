using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de condomínios.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com os condomínios, como criação, atualização, exclusão e recuperação de informações.
/// </remarks>
public interface ICondominiumHandler
{
    /// <summary>
    /// Cria um novo condomínio.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar um novo condomínio com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Condominium"/> contendo as informações do novo condomínio.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Condominium"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Condominium?>> CreateAsync(Condominium request);

    /// <summary>
    /// Atualiza as informações de um condomínio existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de um condomínio existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Condominium"/> contendo as informações atualizadas do condomínio.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Condominium"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Condominium?>> UpdateAsync(Condominium request);

    /// <summary>
    /// Exclui um condomínio existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por excluir um condomínio existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DeleteCondominiumRequest"/> contendo o Id do condomínio a ser excluído.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Condominium"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request);

    /// <summary>
    /// Recupera as informações de um condomínio existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar as informações de um condomínio existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetCondominiumByIdRequest"/> contendo o Id do condomínio a ser recuperado.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Condominium"/> e pode ser nulo se a operação falhar.
    /// </returns>    
    Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request);

    /// <summary>
    /// Recupera todos os condomínios existentes.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar todos os condomínios existentes com base nos parâmetros de paginação fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllCondominiumsRequest"/> contendo os parâmetros de paginação.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Condominium"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Condominium>?>> GetAllAsync(GetAllCondominiumsRequest request);
}