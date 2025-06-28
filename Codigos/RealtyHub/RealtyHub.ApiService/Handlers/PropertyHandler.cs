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
    public async Task<Response<Imovel?>> CreateAsync(Imovel request)
    {
        try
        {
            var property = new Imovel
            {
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Endereco = request.Endereco,
                TipoImovel = request.TipoImovel,
                Quarto = request.Quarto,
                Banheiro = request.Banheiro,
                Area = request.Area,
                Garagem = request.Garagem,
                Novo = request.Novo,
                NumeroRegistro = request.NumeroRegistro,
                RegistroCartorio = request.RegistroCartorio,
                DetalhesTransacao = request.DetalhesTransacao,
                UsuarioId = request.UsuarioId,
                CondominioId = request.CondominioId,
                ExibirNaHome = request.ExibirNaHome,
                VendedorId = request.VendedorId,
                Ativo = true
            };

            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();

            return new Response<Imovel?>(property, 201, "Imóvel criado com sucesso");
        }
        catch
        {
            return new Response<Imovel?>(null, 500, "Não foi possível criar o imóvel");
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
    public async Task<Response<Imovel?>> UpdateAsync(Imovel request)
    {
        try
        {
            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.Ativo);

            if (property is null)
                return new Response<Imovel?>(null, 404, "Imóvel não encontrado");

            property.Titulo = request.Titulo;
            property.Descricao = request.Descricao;
            property.Preco = request.Preco;
            property.Endereco = request.Endereco;
            property.TipoImovel = request.TipoImovel;
            property.Quarto = request.Quarto;
            property.Banheiro = request.Banheiro;
            property.Area = request.Area;
            property.Garagem = request.Garagem;
            property.Novo = request.Novo;
            property.NumeroRegistro = request.NumeroRegistro;
            property.RegistroCartorio = request.RegistroCartorio;
            property.DetalhesTransacao = request.DetalhesTransacao;
            property.UsuarioId = request.UsuarioId;
            property.ExibirNaHome = request.ExibirNaHome;
            property.VendedorId = request.VendedorId;
            property.CondominioId = request.CondominioId;
            property.AtualizadoEm = DateTime.UtcNow;

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();

            return new Response<Imovel?>(property, 200, "Imóvel atualizado com sucesso");
        }
        catch
        {
            return new Response<Imovel?>(null, 500, "Não foi possível atualizar o imóvel");
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
    public async Task<Response<Imovel?>> DeleteAsync(DeletePropertyRequest request)
    {
        try
        {
            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.Id
                                          && p.UsuarioId == request.UserId
                                          && p.Ativo);

            if (property is null)
                return new Response<Imovel?>(null, 404, "Imóvel não encontrado");

            property.Ativo = false;
            property.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Imovel?>(null, 204, "Imóvel deletado com sucesso");
        }
        catch
        {
            return new Response<Imovel?>(null, 500, "Não foi possível deletar o imóvel");
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
    public async Task<Response<Imovel?>> GetByIdAsync(GetPropertyByIdRequest request)
    {
        try
        {
            var query = _context
                .Properties
                .AsNoTracking()
                .Include(p => p.Condominio)
                .Include(p => p.Vendedor)
                .Include(p => p.FotosImovel.Where(photo => photo.Ativo))
                .Where(p => p.Id == request.Id && p.Ativo);

            var property = await query.FirstOrDefaultAsync();

            return property is null
                ? new Response<Imovel?>(null, 404, "Imóvel não encontrado")
                : new Response<Imovel?>(property);
        }
        catch
        {
            return new Response<Imovel?>(null, 500, "Não foi possível retornar o imóvel");
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
    public async Task<PagedResponse<List<Imovel>?>> GetAllAsync(GetAllPropertiesRequest request)
    {
        try
        {
            var query = _context
                .Properties
                .AsNoTracking()
                .Include(p => p.Condominio)
                .Include(p => p.Vendedor)
                .Include(p => p.FotosImovel.Where(photos => photos.Ativo))
                .Where(p => p.Ativo);

            if (!string.IsNullOrEmpty(request.UserId))
                query = query.Where(v => v.UsuarioId == request.UserId);

            if (!string.IsNullOrEmpty(request.FilterBy))
                query = query.FilterByProperty(request.SearchTerm, request.FilterBy);

            query = query.OrderBy(p => p.Titulo);

            var properties = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Imovel>?>(properties, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Imovel>?>(null, 500, "Não foi possível retornar os imóveis");
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
    public async Task<PagedResponse<List<Visita>?>> GetAllViewingsAsync(GetAllViewingsByPropertyRequest request)
    {
        try
        {
            var query = _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .Where(v => v.ImovelId == request.PropertyId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(v => v.DataVisita >= startDate
                                         && v.DataVisita <= endDate);
            }

            query = query.OrderBy(v => v.DataVisita);

            var viewings = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Visita>?>(viewings, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Visita>?>(null, 500, "Não foi possível retornar as visitas");
        }
    }
}