using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de fotos de imóveis.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com as fotos de imóveis, como criação, atualização, exclusão e recuperação de informações.
/// </remarks>
public interface IPropertyPhotosHandler
{
    /// <summary>
    /// Cria uma nova foto de imóvel.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar uma nova foto de imóvel com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="CreatePropertyPhotosRequest"/> contendo as informações da nova foto.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="FotoImovel"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<FotoImovel?>> CreateAsync(CreatePropertyPhotosRequest request);

    /// <summary>
    /// Atualiza as informações de uma foto de imóvel existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de uma foto de imóvel existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="UpdatePropertyPhotosRequest"/> contendo as informações atualizadas da foto.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="FotoImovel"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<List<FotoImovel>?>> UpdateAsync(UpdatePropertyPhotosRequest request);

    /// <summary>
    /// Exclui uma foto de imóvel existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por excluir uma foto de imóvel existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DeletePropertyPhotoRequest"/> contendo o Id da foto a ser excluída.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="FotoImovel"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<FotoImovel?>> DeleteAsync(DeletePropertyPhotoRequest request);

    /// <summary>
    /// Recupera uma foto de imóvel existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar uma foto de imóvel existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllPropertyPhotosByPropertyRequest"/> contendo o Id da foto a ser recuperada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.  
    /// O objeto TData é uma lista do tipo <see cref="FotoImovel"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<List<FotoImovel>?>> GetAllByPropertyAsync(
        GetAllPropertyPhotosByPropertyRequest request);
}