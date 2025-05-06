using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de imóveis.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com os imóveis, como criação, atualização, exclusão e recuperação de informações.
/// </remarks>
public interface IPropertyHandler
{
    /// <summary>
    /// Cria um novo imóvel.
    ///  </summary>
    /// <remarks>
    /// Este método é responsável por criar um novo imóvel com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Property"/> contendo as informações do novo imóvel.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Property"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Property?>> CreateAsync(Property request);

    /// <summary>
    /// Atualiza as informações de um imóvel existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de um imóvel existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Property"/> contendo as informações atualizadas do imóvel.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Property"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Property?>> UpdateAsync(Property request);

    /// <summary>
    /// Exclui um imóvel existente.
    /// </summary>
    /// <remarks> 
    /// Este método é responsável por excluir um imóvel existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DeletePropertyRequest"/> contendo o Id do imóvel a ser excluído.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Property"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request);

    /// <summary>
    /// Recupera um imóvel existente com base no Id fornecido.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por buscar um imóvel existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetPropertyByIdRequest"/> contendo o Id do imóvel a ser recuperado.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> contendo as informações do imóvel.
    /// O objeto TData é do tipo <see cref="Property"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request);

    /// <summary>
    /// Recupera todos os imóveis disponíveis.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por buscar todos os imóveis armazenados no sistema e retornar uma lista com os resultados.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllPropertiesRequest"/> contendo os parâmetros de filtragem.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> contendo uma lista de imóveis.
    /// O objeto TData é uma lista do tipo <see cref="Property"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request);

    /// <summary>
    /// Recupera todos os imóveis disponíveis com base no Id do usuário.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por buscar todos os imóveis armazenados no sistema e retornar uma lista com os resultados.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllViewingsByPropertyRequest"/> contendo os parâmetros de filtragem.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> contendo uma lista de imóveis.
    /// O objeto TData é uma lista do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.
    /// </returns>    
    Task<PagedResponse<List<Viewing>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request);
}