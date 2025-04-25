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
public class CustomerHandler : ICustomerHandler
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CustomerHandler"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados para interação com clientes.</param>
    public CustomerHandler(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Cria um novo cliente no banco de dados.
    /// </summary>
    /// <param name="request">Objeto contendo as informações para criação do cliente.</param>
    /// <returns>Retorna uma resposta com o cliente criado e o código de status.</returns>
    public async Task<Response<Customer?>> CreateAsync(Customer request)
    {
        try
        {
            var isCustomerExists = await _context
                .Customers
                .AnyAsync(c => (c.DocumentNumber == request.DocumentNumber
                               || c.Email == request.Email)
                               && c.UserId == request.UserId);

            if (isCustomerExists)
                return new Response<Customer?>(null, 400, "Cliente já cadastrado");

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                DocumentNumber = request.DocumentNumber,
                Nationality = request.Nationality,
                MaritalStatus = request.MaritalStatus,
                Occupation = request.Occupation,
                PersonType = request.PersonType,
                CustomerType = request.CustomerType,
                Address = request.Address,
                Rg = request.Rg,
                IssuingAuthority = request.IssuingAuthority,
                RgIssueDate = request.RgIssueDate,
                BusinessName = request.BusinessName,
                UserId = request.UserId,
                IsActive = true
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return new Response<Customer?>(customer, 201, "Cliente criado com sucesso");
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível criar o cliente");
        }
    }

    /// <summary>
    /// Atualiza um cliente existente no banco de dados.
    /// </summary>
    /// <param name="request">Objeto contendo as novas informações do cliente.</param>
    /// <returns>Retorna a resposta com o cliente atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Customer?>> UpdateAsync(Customer request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (string.IsNullOrEmpty(c.UserId) || c.UserId == request.UserId)
                                          && c.IsActive);

            if (customer is null)
                return new Response<Customer?>(null, 404, "Cliente não encontrado");

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.DocumentNumber = request.DocumentNumber;
            customer.Nationality = request.Nationality;
            customer.MaritalStatus = request.MaritalStatus;
            customer.Occupation = request.Occupation;
            customer.IssuingAuthority = request.IssuingAuthority;
            customer.RgIssueDate = request.RgIssueDate;
            customer.PersonType = request.PersonType;
            customer.CustomerType = request.CustomerType;
            customer.Address = request.Address;
            customer.Rg = request.Rg;
            customer.BusinessName = request.BusinessName;
            customer.UserId = request.UserId;
            customer.UpdatedAt = DateTime.UtcNow;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return new Response<Customer?>(customer);
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível atualizar o cliente");
        }
    }

    /// <summary>
    /// Realiza a exclusão lógica de um cliente, marcando-o como inativo.
    /// </summary>
    /// <param name="request">Requisição que contém o ID do cliente a ser excluído.</param>
    /// <returns>Retorna a resposta com o status da exclusão ou um erro se não for encontrado.</returns>
    public async Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (string.IsNullOrEmpty(c.UserId) || c.UserId == request.UserId)
                                          && c.IsActive);

            if (customer is null)
                return new Response<Customer?>(null, 404, "Cliente não encontrado");

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Customer?>(null, 204, "Cliente excluído com sucesso");
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível excluir o cliente");
        }
    }

    /// <summary>
    /// Obtém um cliente específico pelo ID.
    /// </summary>
    /// <param name="request">Requisição que contém o ID do cliente desejado.</param>
    /// <returns>Retorna o objeto do cliente ou um erro caso não seja encontrado.</returns>
    public async Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .Include(c => c.Properties)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && (c.UserId == request.UserId || string.IsNullOrEmpty(c.UserId))
                                          && c.IsActive);

            return customer is null
                ? new Response<Customer?>(null, 404, "Cliente não encontrado")
                : new Response<Customer?>(customer);
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível retornar o cliente");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todos os clientes ativos, com opção de filtro por busca.
    /// </summary>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com os clientes ativos ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        try
        {
            var query = _context
                .Customers
                .Include(c => c.Properties)
                .AsNoTracking()
                .Where(c => (c.UserId == request.UserId || string.IsNullOrEmpty(c.UserId))
                            && c.IsActive);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var lowerSearchTerm = request.SearchTerm.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(lowerSearchTerm) ||
                                         c.Email.ToLower().Contains(lowerSearchTerm) ||
                                         c.DocumentNumber.ToLower().Contains(lowerSearchTerm));
            }

            query = query.OrderBy(c => c.Name);

            var customers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Customer>?>(customers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Customer>?>(null, 500, "Não foi possível consultar os clientes");
        }
    }
}