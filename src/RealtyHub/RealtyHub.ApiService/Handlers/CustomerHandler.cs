using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class CustomerHandler(AppDbContext context) : ICustomerHandler
{
    public async Task<Response<Customer?>> CreateAsync(Customer request)
    {
        try
        {
            var isCustomerExists = await context
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
                BusinessName = request.BusinessName,
                UserId = request.UserId,
                IsActive = true
            };
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();

            return new Response<Customer?>(customer, 201, "Cliente criado com sucesso");
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível criar o cliente");
        }
    }

    public async Task<Response<Customer?>> UpdateAsync(Customer request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (customer is null)
                return new Response<Customer?>(null, 404, "Cliente não encontrado");

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.DocumentNumber = request.DocumentNumber;
            customer.Nationality = request.Nationality;
            customer.MaritalStatus = request.MaritalStatus;
            customer.Occupation = request.Occupation;
            customer.PersonType = request.PersonType;
            customer.CustomerType = request.CustomerType;
            customer.Address = request.Address;
            customer.Rg = request.Rg;
            customer.BusinessName = request.BusinessName;
            customer.UserId = request.UserId;
            customer.UpdatedAt = DateTime.UtcNow;

            context.Customers.Update(customer);
            await context.SaveChangesAsync();

            return new Response<Customer?>(customer, message: "Cliente atualizado com sucesso");
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível atualizar o cliente");
        }
    }

    public async Task<Response<Customer?>> DeleteAsync(DeleteCustomerRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            if (customer is null)
                return new Response<Customer?>(null, 404, "Cliente não encontrado");

            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Customer?>(null, 204, "Cliente excluído com sucesso");
        }
        catch
        {
            return new Response<Customer?>(null, 500, "Não foi possível excluir o cliente");
        }
    }

    public async Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .Include(c=>c.Properties)
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

    public async Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        try
        {
            var query = context
                .Customers
                .Include(c => c.Properties)
                .AsNoTracking()
                .Where(c => (c.UserId == request.UserId || string.IsNullOrEmpty(c.UserId))
                            && c.IsActive);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(request.SearchTerm)
                                         || c.Email.Contains(request.SearchTerm)
                                         || c.DocumentNumber.Contains(request.SearchTerm));
            }

            query = query.OrderBy(c => c.Name);

            var customers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Customer>?>(
               customers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Customer>?>(null, 500, "Não foi possível consultar os clientes");
        }
    }
}