using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Properties;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas a imóveis.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IPropertyHandler"/> e fornece
/// métodos para criar, atualizar, deletar e buscar imóveis no banco de dados.
/// </remarks>
public class PropertyHandler : IPropertyHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com imóveis.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nos imóveis.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PropertyHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nos imóveis.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com imóveis.</param>
    public PropertyHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo imóvel no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método adiciona um novo imóvel à base de dados e salva as alterações.
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para criação do imóvel.</param>
    /// <returns>Retorna uma resposta com o imóvel criado e o código de status.</returns>
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

    /// <summary>
    /// Atualiza um imóvel existente no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método atualiza as informações de um imóvel existente e salva as alterações.
    /// </remarks>
    /// <param name="request">Objeto contendo as novas informações do imóvel.</param>
    /// <returns>Retorna a resposta com o imóvel atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Property?>> UpdateAsync(Property request)
    {
        try
        {
            var property = await _context
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

    /// <summary>
    /// Realiza a exclusão lógica de um imóvel, marcando-o como inativo.
    /// </summary>
    /// <remarks>
    /// Este método altera o status de um imóvel para inativo no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do imóvel a ser excluído.</param>
    /// <returns>Retorna a resposta com o status da exclusão ou um erro se não for encontrado.</returns>
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

    /// <summary>
    /// Obtém um imóvel específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método busca um imóvel no banco de dados com base no ID fornecido.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do imóvel desejado.</param>
    /// <returns>Retorna o objeto do imóvel ou um erro caso não seja encontrado.</returns>
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

    /// <summary>
    /// Retorna uma lista paginada de todos os imóveis ativos, com opção de filtro por busca.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todos os imóveis ativos no banco de dados e aplica
    /// filtros de paginação e busca, se fornecidos.</para>    
    /// <para> Caso o filtro não seja fornecido, retorna todos os imóveis ativos.</para>
    /// </remarks>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com os imóveis ativos ou um erro em caso de falha.</returns>
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

            return new PagedResponse<List<Property>?>(properties, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Property>?>(null, 500, "Não foi possível retornar os imóveis");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as visitas de um imóvel específico.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todas as visitas de um imóvel específico no banco de dados e aplica
    /// filtros de paginação e busca, se fornecidos.</para>
    /// <para> Caso o filtro não seja fornecido, retorna todas as visitas do imóvel.</para>
    /// </remarks>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro por data.</param>
    /// <returns>Retorna uma resposta paginada com as visitas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Viewing>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request)
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

            return new PagedResponse<List<Viewing>?>(viewings, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Viewing>?>(null, 500, "Não foi possível retornar as visitas");
        }
    }
}