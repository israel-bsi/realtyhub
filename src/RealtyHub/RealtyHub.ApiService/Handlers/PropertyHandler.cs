using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class PropertyHandler(AppDbContext context) : IPropertyHandler
{
    public async Task<Response<Property?>> CreateAsync(CreatePropertyRequest request)
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
                TransactionsDetails = request.TransactionsDetails,
                IsActive = true
            };

            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            return new Response<Property?>(property, 201, "Imóvel criado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Property?>(null, 500, $"Não foi possível criar o imóvel\n{ex.Message}");
        }
    }

    public async Task<Response<Property?>> UpdateAsync(UpdatePropertyRequest request)
    {
        try
        {
            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive);

            if (property is null)
                return new Response<Property?>(null, 404, "Imóvel não encontrado");

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
            property.TransactionsDetails = request.TransactionsDetails;
            property.UpdatedAt = DateTime.UtcNow;

            context.Properties.Update(property);
            await context.SaveChangesAsync();

            return new Response<Property?>(property, 200, "Imóvel atualizado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Property?>(null, 500, $"Não foi possível atualizar o imóvel\n{ex.Message}");
        }
    }

    public async Task<Response<Property?>> DeleteAsync(DeletePropertyRequest request)
    {
        try
        {
            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive);

            if (property is null)
                return new Response<Property?>(null, 404, "Imóvel não encontrado");

            property.IsActive = false;
            property.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Property?>(property, 200, "Imóvel deletado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Property?>(null, 500, $"Não foi possível deletar o imóvel\n{ex.Message}");
        }
    }

    public async Task<Response<Property?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        try
        {
            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsActive);

            return property is null
                ? new Response<Property?>(null, 404, "Imóvel não encontrado")
                : new Response<Property?>(property);
        }
        catch (Exception ex)
        {
            return new Response<Property?>(null, 500, $"Não foi possível retornar o imóvel\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Property>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        try
        {
            var query = context
                .Properties
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Title);

            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Property>?>(properties, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Property>?>(null, 500, $"Não foi possível retornar os imóveis\n{ex.Message}");
        }
    }
}