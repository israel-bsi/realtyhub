using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

/// <summary>
/// Interface que define os métodos para manipulação de visitas a imóveis.
/// </summary>
/// <remarks>
/// Esta interface é responsável por definir as operações que podem ser realizadas
/// com as visitas de imóveis, como agendamento, reagendamento, conclusão e cancelamento.
/// </remarks>
public interface IViewingHandler
{
    /// <summary>
    /// Agenda uma nova visita.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por agendar uma nova visita a um imóvel com base nas informações fornecidas.
    /// </remarks>
    Task<Response<Viewing?>> ScheduleAsync(Viewing request);

    /// <summary>
    /// Reagenda uma visita existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por reagendar uma visita existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="Viewing"/> contendo as informações da visita a ser reagendada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.
    /// </returns> 
    Task<Response<Viewing?>> RescheduleAsync(Viewing request);

    /// <summary>
    /// Conclui uma visita existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por concluir uma visita existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="DoneViewingRequest"/> contendo as informações da visita a ser concluída.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request);

    /// <summary>
    /// Cancela uma visita existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por cancelar uma visita existente com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="CancelViewingRequest"/> contendo as informações da visita a ser cancelada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request);

    /// <summary>
    /// Recupera uma visita existente.
    /// </summary>
    /// <remarks>
    /// Este método é responsável por recuperar uma visita existente com base no Id fornecido.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetViewingByIdRequest"/> contendo o Id da visita a ser recuperada.</param>
    /// <returns>
    /// Retorna um objeto <see cref="Response{TData}"/> indicando o resultado da operação.
    /// O objeto TData é do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.
    /// </returns>
    Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request);

    /// <summary>
    /// Recupera todas as visitas existentes.
    /// </summary>  
    /// <remarks>
    /// Este método é responsável por recuperar todas as visitas existentes com base nas informações fornecidas.
    /// </remarks>
    /// <param name="request">Instância de <see cref="GetAllViewingsRequest"/> contendo as informações para recuperação das visitas.</param>
    /// <returns>
    /// Retorna um objeto <see cref="PagedResponse{TData}"/> indicando o resultado da operação.
    /// O objeto TData é uma lista do tipo <see cref="Viewing"/> e pode ser nulo se a operação falhar.  
    /// </returns>
    Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request);
}