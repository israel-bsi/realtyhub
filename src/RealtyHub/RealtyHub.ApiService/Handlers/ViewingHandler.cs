using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ViewingHandler(AppDbContext context) : IViewingHandler
{
    public async Task<Response<Viewing?>> CreateAsync(CreateViewingRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.IsActive);

            if (customer is null)
                return new Response<Viewing?>(null, 404, "Cliente não encontrado");

            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.IsActive);

            if (property is null)
                return new Response<Viewing?>(null, 404, "Imóvel não encontrado");

            var viewing = new Viewing
            {
                Date = request.Date,
                ViewingStatus = request.ViewingStatus,
                Customer = customer,
                Property = property
            };

            context.Viewing.Add(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, 201, "Visita agendada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500,$"Não foi possível agendar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> UpdateAsync(UpdateViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            viewing.Date = request.Date;
            viewing.ViewingStatus = request.ViewingStatus;
            viewing.CustomerId = request.CustomerId;
            viewing.PropertyId = request.PropertyId;
            viewing.UpdatedAt = DateTime.UtcNow;

            context.Viewing.Update(viewing);
            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita atualizada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, $"Não foi possível atualizar a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> DeleteAsync(DeleteViewingRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            if (viewing is null)
                return new Response<Viewing?>(null, 404, "Visita não encontrada");

            viewing.IsActive = false;

            await context.SaveChangesAsync();

            return new Response<Viewing?>(viewing, message: "Visita excluída com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, $"Não foi possível excluir a visita\n{ex.Message}");
        }
    }

    public async Task<Response<Viewing?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        try
        {
            var viewing = await context
                .Viewing
                .Include(v => v.Customer)
                .Include(v => v.Property)
                .FirstOrDefaultAsync(v => v.Id == request.Id && v.IsActive);

            return viewing is null
                ? new Response<Viewing?>(null, 404, "Visita não encontrada")
                : new Response<Viewing?>(viewing);
        }
        catch (Exception ex)
        {
            return new Response<Viewing?>(null, 500, $"Não foi possível retornar a visita\n{ex.Message}");
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

            return new PagedResponse<List<Viewing>?>(viewings, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Viewing>?>(null, 500, message: $"Não foi possível retornar as visitas\n{ex.Message}");
        }
    }
}