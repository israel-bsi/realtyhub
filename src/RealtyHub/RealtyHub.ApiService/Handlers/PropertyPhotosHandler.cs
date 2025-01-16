using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class PropertyPhotosHandler(AppDbContext context) : IPropertyPhotosHandler
{
    public async Task<Response<PropertyPhoto?>> CreateAsync(CreatePropertyPhotosRequest request)
    {
        try
        {
            if (request.HttpRequest is null)
                return new Response<PropertyPhoto?>(null, 400,
                    "Requisição inválida");

            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive);

            if (property is null)
                return new Response<PropertyPhoto?>(null, 404,
                    "Imóvel não encontrado");

            context.Attach(property);

            if (!request.HttpRequest.HasFormContentType)
                return new Response<PropertyPhoto?>(null, 400,
                    "Conteúdo do tipo multipart/form-data esperado");

            var form = await request.HttpRequest.ReadFormAsync();
            var files = form.Files;

            if (files.Count == 0)
                return new Response<PropertyPhoto?>(null, 400,
                    "Nenhum arquivo encontrado");

            foreach (var file in files)
            {
                var idPhoto = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{idPhoto}{extension}";
                var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "Sources", "Photos", fileName);

                await using var stream = new FileStream(fullFileName, FileMode.Create);
                await file.CopyToAsync(stream);

                var propertyPhoto = new PropertyPhoto
                {
                    Id = idPhoto,
                    Extension = extension,
                    PropertyId = request.PropertyId,
                    Property = property,
                    IsActive = true,
                    UserId = request.UserId
                };

                context.PropertyPhotos.Add(propertyPhoto);
            }

            await context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 201);
        }
        catch (Exception e)
        {
            return new Response<PropertyPhoto?>(null, 500, e.Message);
        }
    }

    public async Task<Response<PropertyPhoto?>> DeleteAsync(DeletePropertyPhotoRequest request)
    {
        try
        {
            var propertyPhoto = await context
                .PropertyPhotos
                .FirstOrDefaultAsync(pi =>
                    pi.Id == request.Id
                    && pi.UserId == request.UserId
                    && pi.IsActive);

            if (propertyPhoto is null)
                return new Response<PropertyPhoto?>(null, 404,
                    "Foto não encontrada");

            context.Attach(propertyPhoto);

            propertyPhoto.IsActive = false;

            await context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 204);
        }
        catch (Exception e)
        {
            return new Response<PropertyPhoto?>(null, 500, e.Message);
        }
    }

    public async Task<Response<List<PropertyPhoto>?>> GetAllByPropertyAsync(GetAllPropertyPhotosByPropertyRequest request)
    {
        try
        {
            var propertyPhotos = await context
                .PropertyPhotos
                .AsNoTracking()
                .Where(pi =>
                    pi.PropertyId == request.PropertyId
                    && pi.UserId == request.UserId
                    && pi.IsActive)
                .ToListAsync();

            return new Response<List<PropertyPhoto>?>(propertyPhotos);
        }
        catch (Exception e)
        {
            return new Response<List<PropertyPhoto>?>(null, 500, e.Message);
        }
    }
}