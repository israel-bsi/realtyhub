using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de propostas.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com as propostas, como criação, atualização, rejeição, aceitação e recuperação de informações.
/// </remarks>
public interface IOfferHandler
{
    /// <summary>
    /// Cria uma nova propostas.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por criar uma nova proposta com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Offer"/> contendo as informações da nova proposta.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> CreateAsync(Offer request);

    /// <summary>
    /// Atualiza as informações de uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por atualizar as informações de uma proposta existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Offer"/> contendo as informações atualizadas da proposta.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> UpdateAsync(Offer request);

    /// <summary>
    /// Rejeita uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por rejeitar uma proposta existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="RejectOfferRequest"/> contendo o Id da proposta a ser rejeitada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> RejectAsync(RejectOfferRequest request);

    /// <summary>
    /// Aceita uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por aceitar uma proposta existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="AcceptOfferRequest"/> contendo o Id da proposta a ser aceita.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> AcceptAsync(AcceptOfferRequest request);

    /// <summary>
    /// Recupera as informações de uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar as informações de uma proposta existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetOfferByIdRequest"/> contendo o Id da proposta a ser recuperada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> GetByIdAsync(GetOfferByIdRequest request);

    /// <summary>
    /// Recupera as informações de uma proposta existente com base no Id do imóvel.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar as informações de uma proposta existente com base no Id do imóvel fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetOfferAcceptedByProperty"/> contendo o Id do imóvel.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Offer?>> GetAcceptedByProperty(GetOfferAcceptedByProperty request);

    /// <summary>
    /// Recupera todas as propostas de um determinado imóvel.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar todas as propostas de um imóvel específico com base nos parâmetros de paginação fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllOffersByPropertyRequest"/> contendo os parâmetros de paginação.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Offer>?>> GetAllOffersByPropertyAsync(GetAllOffersByPropertyRequest request);

    /// <summary>
    /// Recupera todas as propostas de um determinado cliente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar todas as propostas de um cliente específico com base nos parâmetros de paginação fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllOffersByCustomerRequest"/> contendo os parâmetros de paginação.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Offer>?>> GetAllOffersByCustomerAsync(GetAllOffersByCustomerRequest request);

    /// <summary>
    /// Recupera todas as propostas existentes.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar todas as propostas existentes com base nos parâmetros de paginação fornecidos.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllOffersRequest"/> contendo os parâmetros de paginação.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista de <see cref="Offer"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<PagedResponse<List<Offer>?>> GetAllAsync(GetAllOffersRequest request);
}