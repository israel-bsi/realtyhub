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
    public async Task<Response<Viewing?>> ScheduleAsync(ScheduleViewingRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.IsActive);

            if (customer is null)
                return new Response<Viewing?>(null, 404, 
                    "Cliente não encontrado");

            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.IsActive);

            if (property is null)
                return new Response<Viewing?>(null, 404, 
                    "Imóvel não encontrado");

            var isViewingExits = await context
                .Viewing
                .AnyAsync(v => 
                    v.CustomerId == request.CustomerId 
                    && v.PropertyId == request.PropertyId 
                    && v.IsActive);

            if (isViewingExits)
                return new Response<Viewing?>(null, 400,
                $"Já existe uma visita do cliente {customer.Name} " +
                $"para o imóvel {property.Title}");

            var viewing = new Viewing
            {
                Date = request.Date,
                Status = request.ViewingStatus,
                CustomerId = request.CustomerId,
                Customer = customer,
                PropertyId = request.PropertyId,
                Property = property
            };

            context.Viewing.Add(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, 201, 
                "Visita agendada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, 
                $"Não foi possível agendar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> RescheduleAsync(RescheduleViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, 
                    "Visita não encontrada");

            viewing.Date = request.Date;
            viewing.UpdatedAt = DateTime.UtcNow;

            context.Viewing.Update(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita reagendada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, 
                $"Não foi possível reagendar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> DoneAsync(DoneViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, 
                    "Visita não encontrada");

            switch (viewing.Status)
            {
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400, 
                        "Não é possível finalizar uma visita cancelada");
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400, 
                        "Visita já finalizada");
            }

            viewing.Status = EViewingStatus.Done;

            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita finalizada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, 
                $"Não foi possível finalizar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> CancelAsync(CancelViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, 
                    "Visita não encontrada");

            switch (viewing.Status)
            {
                case EViewingStatus.Done:
                    return new Response<Viewing?>(null, 400, 
                        "Não é possível cancelar uma visita finalizada");
                case EViewingStatus.Canceled:
                    return new Response<Viewing?>(null, 400, 
                        "Visita já cancelada");
            }

            viewing.Status = EViewingStatus.Canceled;

            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: 
                "Visita cancelada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, 
                $"Não foi possível cancelar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            return viewing is null
                ? new Response<Viewing?>(null, 404, "Visita não encontrada")
                : new Response<Viewing?>(viewing);
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, 
                $"Não foi possível retornar a visita\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Viewing>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        try
        {
            var query = context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .Where(v => v.IsActive);

            var viewings = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Viewing>?>(
                viewings, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Viewing>?>(null, 500, 
                message: $"Não foi possível retornar as visitas\n{ex.Message}");
        }
    }
}