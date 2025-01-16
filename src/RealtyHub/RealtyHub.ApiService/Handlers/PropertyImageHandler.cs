using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesImages;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class PropertyImageHandler(AppDbContext context) : IPropertyImageHandler
{
    public async Task<Response<PropertyImage?>> CreateAsync(CreatePropertyImageRequest request)
    {
        try
        {
            if (request.HttpRequest is null)
                return new Response<PropertyImage?>(null, 400,
                    "Requisição inválida");

            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive);

            if (property is null)
                return new Response<PropertyImage?>(null, 404,
                    "Imóvel não encontrado");

            context.Attach(property);

            if (!request.HttpRequest.HasFormContentType)
                return new Response<PropertyImage?>(null, 400,
                    "Conteúdo do tipo multipart/form-data esperado");

            var form = await request.HttpRequest.ReadFormAsync();
            var files = form.Files;

            if (files.Count == 0)
                return new Response<PropertyImage?>(null, 400,
                    "Nenhum arquivo encontrado");

            foreach (var file in files)
            {
                var idImage = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{idImage}{extension}";
                var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "Sources", "Images", fileName);

                await using var stream = new FileStream(fullFileName, FileMode.Create);
                await file.CopyToAsync(stream);

                var propertyImage = new PropertyImage
                {
                    Id = idImage,
                    PropertyId = request.PropertyId,
                    Property = property,
                    IsActive = true,
                    UserId = request.UserId
                };

                context.PropertyImages.Add(propertyImage);
            }

            await context.SaveChangesAsync();

            return new Response<PropertyImage?>(null, 201);
        }
        catch (Exception e)
        {
            return new Response<PropertyImage?>(null, 500, e.Message);
        }
    }

    public async Task<Response<PropertyImage?>> DeleteAsync(DeletePropertyImageRequest request)
    {
        try
        {
            var propertyImage = await context
                .PropertyImages
                .FirstOrDefaultAsync(pi =>
                    pi.Id == request.ImageId
                    && pi.UserId == request.UserId
                    && pi.IsActive);

            if (propertyImage is null)
                return new Response<PropertyImage?>(null, 404,
                    "Imagem não encontrada");

            context.Attach(propertyImage);

            propertyImage.IsActive = false;

            await context.SaveChangesAsync();

            return new Response<PropertyImage?>(null, 204);
        }
        catch (Exception e)
        {
            return new Response<PropertyImage?>(null, 500, e.Message);
        }
    }

    public async Task<Response<List<PropertyImage>?>> GetAllByPropertyAsync(GetAllPropertyImagesByPropertyRequest request)
    {
        try
        {
            var propertyImages = await context
                .PropertyImages
                .AsNoTracking()
                .Where(pi =>
                    pi.PropertyId == request.PropertyId
                    && pi.UserId == request.UserId
                    && pi.IsActive)
                .ToListAsync();

            return new Response<List<PropertyImage>?>(propertyImages);
        }
        catch (Exception e)
        {
            return new Response<List<PropertyImage>?>(null, 500, e.Message);
        }
    }
}