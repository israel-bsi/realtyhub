using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas a clientes.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="ICustomerHandler"/> e fornece
/// métodos para criar, atualizar, deletar e buscar clientes no banco de dados.
/// </remarks>
public class CustomerHandler : ICustomerHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com clientes.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nos clientes.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CustomerHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nos clientes.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com clientes.</param>
    public CustomerHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo cliente no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método adiciona um novo cliente à base de dados e salva as alterações.    
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para criação do cliente.</param>
    /// <returns>Retorna uma resposta com o cliente criado ou um erro se o cliente já existir.</returns>
    public async Task<Response<Cliente?>> CreateAsync(Cliente request)
    {
        try
        {
            var isCustomerExists = await _context
                .Customers
                .AnyAsync(c => (c.NumeroDocumento == request.NumeroDocumento
                               || c.Email == request.Email)
                               && c.UsuarioId == request.UsuarioId);

            if (isCustomerExists)
                return new Response<Cliente?>(null, 400, "Cliente já cadastrado");

            var customer = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                NumeroDocumento = request.NumeroDocumento,
                Nacionalidade = request.Nacionalidade,
                TipoStatusCivil = request.TipoStatusCivil,
                Ocupacao = request.Ocupacao,
                TipoPessoa = request.TipoPessoa,
                TipoCliente = request.TipoCliente,
                Endereco = request.Endereco,
                Rg = request.Rg,
                AutoridadeEmissora = request.AutoridadeEmissora,
                DataEmissaoRg = request.DataEmissaoRg,
                NomeFantasia = request.NomeFantasia,
                UsuarioId = request.UsuarioId,
                Ativo = true
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return new Response<Cliente?>(customer, 201, "Cliente criado com sucesso");
        }
        catch
        {
            return new Response<Cliente?>(null, 500, "Não foi possível criar o cliente");
        }
    }

    /// <summary>
    /// Atualiza um cliente existente no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método atualiza as informações de um cliente existente no banco de dados.    
    /// </remarks>
    /// <param name="request">Objeto contendo as novas informações do cliente.</param>
    /// <returns>Retorna a resposta com o cliente atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Cliente?>> UpdateAsync(Cliente request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (string.IsNullOrEmpty(c.UsuarioId) || c.UsuarioId == request.UsuarioId)
                                          && c.Ativo);

            if (customer is null)
                return new Response<Cliente?>(null, 404, "Cliente não encontrado");

            customer.Nome = request.Nome;
            customer.Email = request.Email;
            customer.Telefone = request.Telefone;
            customer.NumeroDocumento = request.NumeroDocumento;
            customer.Nacionalidade = request.Nacionalidade;
            customer.TipoStatusCivil = request.TipoStatusCivil;
            customer.Ocupacao = request.Ocupacao;
            customer.AutoridadeEmissora = request.AutoridadeEmissora;
            customer.DataEmissaoRg = request.DataEmissaoRg;
            customer.TipoPessoa = request.TipoPessoa;
            customer.TipoCliente = request.TipoCliente;
            customer.Endereco = request.Endereco;
            customer.Rg = request.Rg;
            customer.NomeFantasia = request.NomeFantasia;
            customer.UsuarioId = request.UsuarioId;
            customer.AtualizadoEm = DateTime.UtcNow;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return new Response<Cliente?>(customer);
        }
        catch
        {
            return new Response<Cliente?>(null, 500, "Não foi possível atualizar o cliente");
        }
    }

    /// <summary>
    /// Realiza a exclusão lógica de um cliente, marcando-o como inativo.
    /// </summary>
    /// <remarks>
    /// Este método altera o status de um cliente para inativo no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do cliente a ser excluído.</param>
    /// <returns>Retorna a resposta com o status da exclusão ou um erro se não for encontrado.</returns>
    public async Task<Response<Cliente?>> DeleteAsync(DeleteCustomerRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (string.IsNullOrEmpty(c.UsuarioId) || c.UsuarioId == request.UserId)
                                          && c.Ativo);

            if (customer is null)
                return new Response<Cliente?>(null, 404, "Cliente não encontrado");

            customer.Ativo = false;
            customer.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Cliente?>(null, 204, "Cliente excluído com sucesso");
        }
        catch
        {
            return new Response<Cliente?>(null, 500, "Não foi possível excluir o cliente");
        }
    }

    /// <summary>
    /// Obtém um cliente específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método busca um cliente no banco de dados com base no ID fornecido.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do cliente desejado.</param>
    /// <returns>Retorna o objeto do cliente ou um erro caso não seja encontrado.</returns>
    public async Task<Response<Cliente?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .Include(c => c.Imoveis)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (c.UsuarioId == request.UserId || string.IsNullOrEmpty(c.UsuarioId))
                                          && c.Ativo);

            return customer is null
                ? new Response<Cliente?>(null, 404, "Cliente não encontrado")
                : new Response<Cliente?>(customer);
        }
        catch
        {
            return new Response<Cliente?>(null, 500, "Não foi possível retornar o cliente");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todos os clientes ativos, com opção de filtro por busca.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todos os clientes ativos no banco de dados e aplica
    /// filtros de busca e paginação. </para>
    /// <para> Caso nenhum filtro seja fornecido, retorna todos os clientes ativos. </para>
    /// </remarks>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com os clientes ativos ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Cliente>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        try
        {
            var query = _context
                .Customers
                .Include(c => c.Imoveis)
                .AsNoTracking()
                .Where(c => (c.UsuarioId == request.UserId || string.IsNullOrEmpty(c.UsuarioId))
                            && c.Ativo);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var lowerSearchTerm = request.SearchTerm.ToLower();
                query = query.Where(c => c.Nome.ToLower().Contains(lowerSearchTerm) ||
                                         c.Email.ToLower().Contains(lowerSearchTerm) ||
                                         c.NumeroDocumento.ToLower().Contains(lowerSearchTerm));
            }

            query = query.OrderBy(c => c.Nome);

            var customers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Cliente>?>(customers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Cliente>?>(null, 500, "Não foi possível consultar os clientes");
        }
    }
}