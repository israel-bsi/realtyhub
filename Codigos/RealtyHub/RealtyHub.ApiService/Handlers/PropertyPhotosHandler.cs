using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.PropertiesPhotos;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas às fotos de imóveis.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IPropertyPhotosHandler"/> e fornece
/// métodos para criar, atualizar, deletar e buscar fotos de imóveis no banco de dados.
/// </remarks>
public class PropertyPhotosHandler : IPropertyPhotosHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com fotos dos imóveis.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nas fotos dos imóveis.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PropertyPhotosHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nas fotos dos imóveis.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com as fotos de imóveis.</param>
    public PropertyPhotosHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria novas fotos para um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método adiciona novas fotos à base de dados e salva as alterações.
    /// </remarks>
    /// <param name="request">Requisição contendo as informações e os arquivos para criação das fotos.</param>
    /// <returns>Retorna uma resposta indicando o sucesso ou falha da operação.</returns>
    public async Task<Response<PropertyPhoto?>> CreateAsync(CreatePropertyPhotosRequest request)
    {
        try
        {
            if (request.HttpRequest is null)
                return new Response<PropertyPhoto?>(null, 400, "Requisição inválida");

            var property = await _context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.Id == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive);

            if (property is null)
                return new Response<PropertyPhoto?>(null, 404, "Imóvel não encontrado");

            _context.Attach(property);

            if (!request.HttpRequest.HasFormContentType)
                return new Response<PropertyPhoto?>(null, 400, "Conteúdo do tipo multipart/form-data esperado");

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
                var fullFileName = Path.Combine(Configuration.PhotosPath, fileName);

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
                await _context.PropertyPhotos
                    .Where(pi =>
                        pi.PropertyId == request.PropertyId
                        && pi.UserId == request.UserId
                        && pi.IsActive
                        && pi.IsThumbnail)
                    .ForEachAsync(pi =>
                    {
                        _context.Attach(pi);
                        pi.IsThumbnail = false;
                        pi.UpdatedAt = DateTime.UtcNow;
                    });
            }

            await _context.PropertyPhotos.AddRangeAsync(photosToCreate);
            _context.PropertyPhotos.UpdateRange(photosToUpdate);
            await _context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 201);
        }
        catch
        {
            return new Response<PropertyPhoto?>(null, 500, "Não foi possível criar as fotos");
        }
    }

    /// <summary>
    /// Atualiza as informações de fotos de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método atualiza as informações de fotos existentes no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição contendo as informações das fotos a serem atualizadas.</param>
    /// <returns>Retorna uma resposta indicando o sucesso ou falha da operação.</returns>
    public async Task<Response<List<PropertyPhoto>?>> UpdateAsync(UpdatePropertyPhotosRequest request)
    {
        try
        {
            if (request.Photos.Any(p => p.IsThumbnail))
            {
                await _context.PropertyPhotos
                    .Where(pi =>
                        pi.PropertyId == request.PropertyId
                        && pi.UserId == request.UserId
                        && pi.IsActive
                        && pi.IsThumbnail)
                    .ForEachAsync(pi =>
                    {
                        _context.Attach(pi);
                        pi.IsThumbnail = false;
                        pi.UpdatedAt = DateTime.UtcNow;
                    });
            }

            foreach (var photo in request.Photos)
            {
                var existingEntity = await _context
                    .PropertyPhotos
                    .FirstOrDefaultAsync(pi =>
                        pi.Id == photo.Id
                        && pi.UserId == request.UserId
                        && pi.IsActive);

                if (existingEntity is null)
                    continue;

                _context.Attach(existingEntity);

                existingEntity.IsThumbnail = photo.IsThumbnail;
                existingEntity.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new Response<List<PropertyPhoto>?>();
        }
        catch
        {
            return new Response<List<PropertyPhoto>?>(null, 500, "Não foi possível atualizar as fotos");
        }
    }

    /// <summary>
    /// Exclui uma foto de um imóvel.
    /// </summary>
    /// <remarks>
    /// Este método marca uma foto como inativa no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da foto a ser excluída.</param>
    /// <returns>Retorna uma resposta indicando o sucesso ou falha da operação.</returns>
    public async Task<Response<PropertyPhoto?>> DeleteAsync(DeletePropertyPhotoRequest request)
    {
        try
        {
            var propertyPhoto = await _context
                .PropertyPhotos
                .FirstOrDefaultAsync(pi =>
                    pi.Id == request.Id
                    && pi.UserId == request.UserId
                    && pi.IsActive);

            if (propertyPhoto is null)
                return new Response<PropertyPhoto?>(null, 404, "Foto não encontrada");

            _context.Attach(propertyPhoto);

            propertyPhoto.IsActive = false;
            propertyPhoto.IsThumbnail = false;
            propertyPhoto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<PropertyPhoto?>(null, 204, "Foto excluída com sucesso");
        }
        catch
        {
            return new Response<PropertyPhoto?>(null, 500, "Não foi possível deletar a foto");
        }
    }

    /// <summary>
    /// Obtém todas as fotos de um imóvel específico.
    /// </summary>
    /// <remarks>
    /// Este método busca todas as fotos de um imóvel no banco de dados com base no ID do imóvel.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID do imóvel.</param>
    /// <returns>Retorna uma lista de fotos do imóvel ou um erro em caso de falha.</returns>
    public async Task<Response<List<PropertyPhoto>?>> GetAllByPropertyAsync(
       GetAllPropertyPhotosByPropertyRequest request)
    {
        try
        {
            var propertyPhotos = await _context
                .PropertyPhotos
                .AsNoTracking()
                .Where(p =>
                    p.PropertyId == request.PropertyId
                    && p.UserId == request.UserId
                    && p.IsActive)
                .OrderBy(p => p.IsThumbnail)
                .ToListAsync();

            return new Response<List<PropertyPhoto>?>(propertyPhotos);
        }
        catch
        {
            return new Response<List<PropertyPhoto>?>(null, 500, "Não foi possível buscar a foto");
        }
    }
}