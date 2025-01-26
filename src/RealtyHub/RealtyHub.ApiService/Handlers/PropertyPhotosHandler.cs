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
                return new Response<PropertyPhoto?>(null, 400, "Requisição inválida");

            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive);

            if (property is null)
                return new Response<PropertyPhoto?>(null, 404, "Imóvel não encontrado");

            context.Attach(property);

            if (!request.HttpRequest.HasFormContentType)
                return new Response<PropertyPhoto?>(null, 400,
                   "Conteúdo do tipo multipart/form-data esperado");

            var form = await request.HttpRequest.ReadFormAsync();
            var files = form.Files;

            if (files.Count == 0)
                return new Response<PropertyPhoto?>(null, 400, "Nenhum arquivo encontrado");

            var photosToCreate = new List<PropertyPhoto>();
            var photosToUpdate = new List<PropertyPhoto>();

            foreach (var file in files)
            {
                var id = file.Headers["Id"].FirstOrDefault();
                var isThumbnail = bool.Parse(file.Headers["IsThumbnail"].FirstOrDefault() ?? "false");
                var idPhoto = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{idPhoto}{extension}";
                var currentDirectory = Directory.GetCurrentDirectory();
                var fullFileName = Path.Combine(currentDirectory, "Sources", "Photos", fileName);

                await using var stream = new FileStream(fullFileName, FileMode.Create);
                await file.CopyToAsync(stream);

                var propertyPhoto = new PropertyPhoto
                {
                    Id = idPhoto,
                    Extension = extension,
                    IsThumbnail = isThumbnail,
                    PropertyId = request.PropertyId,
                    Property = property,
                    IsActive = true,
                    UserId = request.UserId
                };
                if (string.IsNullOrEmpty(id))
                    photosToCreate.Add(propertyPhoto);
                else
                    photosToUpdate.Add(propertyPhoto);
            }

            if (photosToCreate.Any(p => p.IsThumbnail))
            {
                await context.PropertyPhotos
                    .Where(pi =>
                        pi.PropertyId == request.PropertyId
                        && pi.UserId == request.UserId
                        && pi.IsActive
                        && pi.IsThumbnail)
                    .ForEachAsync(pi =>
                    {
                        context.Attach(pi);
                        pi.IsThumbnail = false;
                        pi.UpdatedAt = DateTime.UtcNow;
                    });
            }

            await context.PropertyPhotos.AddRangeAsync(photosToCreate);
            context.PropertyPhotos.UpdateRange(photosToUpdate);
            await context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 201);
        }
        catch
        {
            return new Response<PropertyPhoto?>(null, 500, "Não foi possível criar as fotos");
        }
    }

    public async Task<Response<List<PropertyPhoto>?>> UpdateAsync(UpdatePorpertyPhotosRequest request)
    {
        try
        {
            if (request.Photos.Any(p => p.IsThumbnail))
            {
                await context.PropertyPhotos
                    .Where(pi =>
                        pi.PropertyId == request.PropertyId
                        && pi.UserId == request.UserId
                        && pi.IsActive
                        && pi.IsThumbnail)
                    .ForEachAsync(pi =>
                    {
                        context.Attach(pi);
                        pi.IsThumbnail = false;
                        pi.UpdatedAt = DateTime.UtcNow;
                    });
            }

            foreach (var photo in request.Photos)
            {
                var existingEntity = await context
                    .PropertyPhotos
                    .FirstOrDefaultAsync(pi =>
                        pi.Id == photo.Id
                        && pi.UserId == request.UserId
                        && pi.IsActive);

                if (existingEntity is null)
                    continue;

                context.Attach(existingEntity);

                existingEntity.IsThumbnail = photo.IsThumbnail;
                existingEntity.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return new Response<List<PropertyPhoto>?>();
        }
        catch
        {
            return new Response<List<PropertyPhoto>?>(null, 500, "Não foi possível atualizar as fotos");
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
                return new Response<PropertyPhoto?>(null, 404, "Foto não encontrada");

            context.Attach(propertyPhoto);

            propertyPhoto.IsActive = false;
            propertyPhoto.IsThumbnail = false;
            propertyPhoto.UpdatedAt = DateTime.UtcNow; 

            await context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 204, "Foto excluída com sucesso");
        }
        catch
        {
            return new Response<PropertyPhoto?>(null, 500, "Não foi possível deletar a foto");
        }
    }

    public async Task<Response<List<PropertyPhoto>?>> GetAllByPropertyAsync(
       GetAllPropertyPhotosByPropertyRequest request)
    {
        try
        {
            var propertyPhotos = await context
                .PropertyPhotos
                .AsNoTracking()
                .Where(p =>
                    p.PropertyId == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive)
                .OrderBy(p=>p.IsThumbnail)
                .ToListAsync();

            return new Response<List<PropertyPhoto>?>(propertyPhotos);
        }
        catch
        {
            return new Response<List<PropertyPhoto>?>(null, 500, "Não foi possível buscar a foto");
        }
    }
}