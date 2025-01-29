using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ViewingHandler(AppDbContext context) : IViewingHandler
{
    public async Task<Response<Viewing?>> ScheduleAsync(Viewing request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.BuyerId
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            if (customer is null)
                return new Response<Viewing?>(null, 404, "Cliente não encontrado");

            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && p.UserId == request.UserId
                                          && p.IsActive);

            if (property is null)
                return new Response<Viewing?>(null, 404, "Imóvel não encontrado");

            var isViewingExist = await context
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

            context.Viewing.Add(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, 201, "Visita agendada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível agendar a visita");
        }
    }

    public async Task<Response<Viewing?>> RescheduleAsync(Viewing request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                         && v.UserId == request.UserId);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400,
                       "Não é possível reagendar uma visita cancelada");
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400,
                        "Não é possível reagendar uma visita finalizada");
            }

            viewing.ViewingDate = request.ViewingDate;
            viewing.UpdatedAt = DateTime.UtcNow;

            context.Viewing.Update(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita reagendada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível reagendar a visita");
        }
    }

    public async Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && v.UserId == request.UserId);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400,
                        "Não é possível finalizar uma visita cancelada");
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400,
                        "Visita já finalizada");
            }

            viewing.ViewingStatus = EViewingStatus.Done;
            viewing.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita finalizada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível finalizar a visita");
        }
    }

    public async Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && v.UserId == request.UserId);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            switch (viewing.ViewingStatus)
            {
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400,
                        "Não é possível cancelar uma visita finalizada");
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400,
                        "Visita já cancelada");
            }

            viewing.ViewingStatus = EViewingStatus.Canceled;
            viewing.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita cancelada com sucesso");
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível cancelar a visita");
        }
    }

    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .ThenInclude(p => p!.Seller)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && v.UserId == request.UserId);

            return viewing is null
                ? new Response<Viewing?>(null, 404, "Visita não encontrada")
                : new Response<Viewing?>(viewing);
        }
        catch
        {
            return new Response<Viewing?>(null, 500, "Não foi possível retornar a visita");
        }
    }

    public async Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        try
        {
            var query = context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .ThenInclude(p => p!.Seller)
                .Where(v => v.UserId == request.UserId);

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

            var count = await query.CountAsync();

            return new PagedResponse<List<Viewing>?>(
                viewings, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Viewing>?>(null, 500,
              message: "Não foi possível retornar as visitas");
        }
    }
}