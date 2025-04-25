using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas às visitas de imóveis.
/// </summary>
public class ViewingHandler : IViewingHandler
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ViewingHandler"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados para interação com as visitas.</param>
    public ViewingHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Agenda uma nova visita para um imóvel.
    /// </summary>
    /// <param name="request">Objeto contendo as informações para agendamento da visita.</param>
    /// <returns>Retorna uma resposta com a visita agendada ou um erro em caso de falha.</returns>
    public async Task<Response<Viewing?>> ScheduleAsync(Viewing request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.BuyerId
                                          && (string.IsNullOrEmpty(c.UserId) || c.UserId == request.UserId)
                                          && c.IsActive);

            if (customer is null)
                return new Response<Viewing?>(null, 404, "Cliente não encontrado");

            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && (string.IsNullOrEmpty(p.UserId) || p.UserId == request.UserId)
                                          && p.IsActive);

            if (property is null)
                return new Response<Viewing?>(null, 404, "Imóvel não encontrado");

            var isViewingExist = await _context
                .Viewing
                .AnyAsync(v => v.PropertyId == request.PropertyId
                               && v.ViewingDate == request.ViewingDate);

            if (isViewingExist)
                return new Response<Viewing?>(null, 400, "Visita já agendada para esta data");

            var viewing = new Viewing
            {
                ViewingDate = request.ViewingDate,
                ViewingStatus = request.ViewingStatus,
                BuyerId = request.BuyerId,
                Buyer = customer,
                PropertyId = request.PropertyId,
                Property = property,
                UserId = request.UserId
            };

            _context.Viewing.Add(viewing);
            await _context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, 201, "Visita agendada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível agendar a visita");
        }
    }

    /// <summary>
    /// Reagenda uma visita existente.
    /// </summary>
    /// <param name="request">Objeto contendo as informações para reagendamento da visita.</param>
    /// <returns>Retorna uma resposta com a visita reagendada ou um erro em caso de falha.</returns>
    public async Task<Response<Viewing?>> RescheduleAsync(Viewing request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UserId) || v.UserId == request.UserId));

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400, "Não é possível reagendar uma visita cancelada");
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400, "Não é possível reagendar uma visita finalizada");
            }

            viewing.ViewingDate = request.ViewingDate;
            viewing.UpdatedAt = DateTime.UtcNow;

            _context.Viewing.Update(viewing);
            await _context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita reagendada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível reagendar a visita");
        }
    }

    /// <summary>
    /// Finaliza uma visita existente.
    /// </summary>
    /// <param name="request">Requisição contendo o ID da visita a ser finalizada.</param>
    /// <returns>Retorna uma resposta com a visita finalizada ou um erro em caso de falha.</returns>
    public async Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UserId) || v.UserId == request.UserId));

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400, "Não é possível finalizar uma visita cancelada");
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400, "Visita já finalizada");
            }

            viewing.ViewingStatus = EViewingStatus.Done;
            viewing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita finalizada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível finalizar a visita");
        }
    }

    /// <summary>
    /// Cancela uma visita existente.
    /// </summary>
    /// <param name="request">Requisição contendo o ID da visita a ser cancelada.</param>
    /// <returns>Retorna uma resposta com a visita cancelada ou um erro em caso de falha.</returns>
    public async Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UserId) || v.UserId == request.UserId));

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400, "Não é possível cancelar uma visita finalizada");
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400, "Visita já cancelada");
            }

            viewing.ViewingStatus = EViewingStatus.Canceled;
            viewing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita cancelada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível cancelar a visita");
        }
    }

    /// <summary>
    /// Obtém uma visita específica pelo ID.
    /// </summary>
    /// <param name="request">Requisição contendo o ID da visita desejada.</param>
    /// <returns>Retorna a visita ou um erro caso não seja encontrada.</returns>
    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .ThenInclude(p => p!.Seller)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UserId) || v.UserId == request.UserId));

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            viewing.ViewingDate = viewing.ViewingDate?.ToLocalTime();
            return new Response<Viewing?>(viewing);
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível retornar a visita");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as visitas.
    /// </summary>
    /// <param name="request">Requisição contendo parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com as visitas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        try
        {
            var query = _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .ThenInclude(p => p!.Seller)
                .Where(v => string.IsNullOrEmpty(v.UserId) || v.UserId == request.UserId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(v => v.ViewingDate >= startDate
                                         && v.ViewingDate <= endDate);
            }

            query = query.OrderBy(v => v.ViewingDate);

            var viewings = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var viewing in viewings)
            {
                viewing.ViewingDate = viewing.ViewingDate?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Viewing>?>(
                viewings, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Viewing>?>(null, 500, "Não foi possível retornar as visitas");
        }
    }
}
