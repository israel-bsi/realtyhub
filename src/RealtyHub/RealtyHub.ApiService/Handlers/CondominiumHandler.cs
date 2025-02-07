using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;
using System.Linq.Dynamic.Core;

namespace RealtyHub.ApiService.Handlers;

public class CondominiumHandler(AppDbContext context) : ICondominiumHandler
{
    public async Task<Response<Condominium?>> CreateAsync(Condominium request)
    {
        try
        {
            var condominium = new Condominium
            {
                Name = request.Name,
                Address = request.Address,
                Units = request.Units,
                Floors = request.Floors,
                HasElevator = request.HasElevator,
                HasSwimmingPool = request.HasSwimmingPool,
                HasPartyRoom = request.HasPartyRoom,
                HasPlayground = request.HasPlayground,
                HasFitnessRoom = request.HasFitnessRoom,
                CondominiumValue = request.CondominiumValue,
                IsActive = true
            };

            await context.Condominiums.AddAsync(condominium);
            await context.SaveChangesAsync();

            return new Response<Condominium?>(condominium, 201);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível criar o condomínio");
        }
    }

    public async Task<Response<Condominium?>> UpdateAsync(Condominium request)
    {
        try
        {
            var condominuim = await context
                .Condominiums
                .FirstOrDefaultAsync(c=>c.Id == request.Id && c.IsActive);

            if (condominuim is null)
                return new Response<Condominium?>(null, 404, "Condomínio não encontrado");

            condominuim.Name = request.Name;
            condominuim.Address = request.Address;
            condominuim.Units = request.Units;
            condominuim.Floors = request.Floors;
            condominuim.HasElevator = request.HasElevator;
            condominuim.HasSwimmingPool = request.HasSwimmingPool;
            condominuim.HasPartyRoom = request.HasPartyRoom;
            condominuim.HasPlayground = request.HasPlayground;
            condominuim.HasFitnessRoom = request.HasFitnessRoom;
            condominuim.CondominiumValue = request.CondominiumValue;
            condominuim.UpdatedAt = DateTime.UtcNow;

            context.Condominiums.Update(condominuim);
            await context.SaveChangesAsync();

            return new Response<Condominium?>(condominuim);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível atualizar o condomínio");
        }
    }

    public async Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request)
    {
        try
        {
            var condominuim = await context
                .Condominiums
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);
            if (condominuim is null)
                return new Response<Condominium?>(null, 404, "Condomínio não encontrado");

            condominuim.IsActive = false;
            condominuim.UpdatedAt = DateTime.UtcNow;

            context.Condominiums.Update(condominuim);
            await context.SaveChangesAsync();

            return new Response<Condominium?>(condominuim);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível deletar o condomínio");
        }
    }

    public async Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request)
    {
        try
        {
            var condominuim = await context
                .Condominiums
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);
            return condominuim is null
                ? new Response<Condominium?>(null, 404, "Condomínio não encontrado")
                : new Response<Condominium?>(condominuim);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível buscar o condomínio");
        }
    }

    public async Task<PagedResponse<List<Condominium>?>> GetAllAsync(GetAllCondominiumsRequest request)
    {
        try
        {
            var query = context
                .Condominiums
                .AsNoTracking()
                .Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(request.SearchTerm) && !string.IsNullOrEmpty(request.FilterBy))
            {
                var propertyType = typeof(Condominium).GetProperty(request.FilterBy)?.PropertyType;

                if (propertyType != null)
                {
                    if (propertyType == typeof(string))
                    {
                        query = query.Where($"{request.FilterBy}.ToLower().Contains(@0.ToLower())", request.SearchTerm);
                    }
                    else if (propertyType.IsValueType)
                    {
                        var searchValue = Convert.ChangeType(request.SearchTerm, propertyType);
                        query = query.Where($"{request.FilterBy} == @0", searchValue);
                    }
                }
            }

            var count = await query.CountAsync();

            var condominiums = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<List<Condominium>?>(condominiums, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Condominium>?>(null, 500, "Não foi possível buscar os condomínios");
        }
    }
}