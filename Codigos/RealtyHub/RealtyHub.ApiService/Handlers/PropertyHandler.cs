using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class PropertyHandler : IPropertyHandler
{
    private readonly AppDbContext _context;

    public PropertyHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Response<Property?>> CreateAsync(Property request)
    {
        try
        {
            var property = new Property
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Address = request.Address,
                PropertyType = request.PropertyType,
                Bedroom = request.Bedroom,
                Bathroom = request.Bathroom,
                Area = request.Area,
                Garage = request.Garage,
                IsNew = request.IsNew,
                RegistryNumber = request.RegistryNumber,
                RegistryRecord = request.RegistryRecord,
                TransactionsDetails = request.TransactionsDetails,
                UserId = request.UserId,
                CondominiumId = request.CondominiumId,
                ShowInHome = request.ShowInHome,
                SellerId = request.SellerId,
                IsActive = true
            };

            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();

            return new Response<Property?>(property, 201, "Imóvel criado com sucesso");
        }
        catch
        {
            return new Response<Property?>(null, 500, "Não foi possível criar o imóvel");
        }
    }

    public async Task<Response<Property?>> UpdateAsync(Property request)
    {
        try
        {
            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive);

            if (property is null)
                return new Response<Property?>(null, 404,
                    "Imóvel não encontrado");

            property.Title = request.Title;
            property.Description = request.Description;
            property.Price = request.Price;
            property.Address = request.Address;
            property.PropertyType = request.PropertyType;
            property.Bedroom = request.Bedroom;
            property.Bathroom = request.Bathroom;
            property.Area = request.Area;
            property.Garage = request.Garage;
            property.IsNew = request.IsNew;
            property.RegistryNumber = request.RegistryNumber;
            property.RegistryRecord = request.RegistryRecord;
            property.TransactionsDetails = request.TransactionsDetails;
            property.UserId = request.UserId;
            property.ShowInHome = request.ShowInHome;
            property.SellerId = request.SellerId;
            property.CondominiumId = request.CondominiumId;
            property.UpdatedAt = DateTime.UtcNow;

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            return new Response<Property?>(property, 200, "Imóvel atualizado com sucesso");
        }
        catch
        {
            return new Response<Property?>(null, 500, "Não foi possível atualizar o imóvel");
        }
    }

    public async Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request)
    {
        try
        {
            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id
                                          && p.UserId == request.UserId
                                          && p.IsActive);

            if (property is null)
                return new Response<Property?>(null, 404, "Imóvel não encontrado");

            property.IsActive = false;
            property.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Property?>(null, 204, "Imóvel deletado com sucesso");
        }
        catch
        {
            return new Response<Property?>(null, 500, "Não foi possível deletar o imóvel");
        }
    }

    public async Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        try
        {
            var query = _context
                .Properties
                .AsNoTracking()
                .Include(p => p.Condominium)
                .Include(p => p.Seller)
                .Include(p => p.PropertyPhotos.Where(photo => photo.IsActive))
                .Where(p => p.Id == request.Id && p.IsActive);

            var property = await query.FirstOrDefaultAsync();

            return property is null
                ? new Response<Property?>(null, 404, "Imóvel não encontrado")
                : new Response<Property?>(property);
        }
        catch
        {
            return new Response<Property?>(null, 500, "Não foi possível retornar o imóvel");
        }
    }

    public async Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        try
        {
            var query = _context
                .Properties
                .AsNoTracking()
                .Include(p => p.Condominium)
                .Include(p => p.Seller)
                .Include(p => p.PropertyPhotos.Where(photos => photos.IsActive))
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(request.UserId))
                query = query.Where(v => v.UserId == request.UserId);

            if (!string.IsNullOrEmpty(request.FilterBy)) 
                query = query.FilterByProperty(request.SearchTerm, request.FilterBy);

            query = query.OrderBy(p => p.Title);

            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Property>?>(
               properties, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Property>?>(null, 500, "Não foi possível retornar os imóveis");
        }
    }

    public async Task<PagedResponse<List<Viewing>?>> GetAllViewingsAsync(
        GetAllViewingsByPropertyRequest request)
    {
        try
        {
            var query = _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Buyer)
                .Include(v => v.Property)
                .Where(v => v.PropertyId == request.PropertyId);

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
            return new PagedResponse<List<Viewing>?>(null, 500, "Não foi possível retornar as visitas");
        }
    }
}