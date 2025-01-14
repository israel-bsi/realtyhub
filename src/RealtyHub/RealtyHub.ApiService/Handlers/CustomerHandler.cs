﻿using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Customers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class CustomerHandler(AppDbContext context) : ICustomerHandler
{
    public async Task<Response<Customer?>> CreateAsync(CreateCustomerRequest request)
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
                CustomerType = request.CustomerType,
                Address = request.Address,
                Rg = request.Rg,
                BusinessName = request.BusinessName,
                UserId = request.UserId,
                IsActive = true
            };
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();

            return new Response<Customer?>(customer, 201, 
                "Cliente criado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Customer?>(null, 500, 
                $"Não foi possível criar o cliente\n{ex.Message}");
        }
    }

    public async Task<Response<Customer?>> UpdateAsync(UpdateCustomerRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (customer is null)
                return new Response<Customer?>(null, 404, 
                    "Cliente não encontrado");

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Phone = request.Phone;
            customer.DocumentNumber = request.DocumentNumber;
            customer.CustomerType = request.CustomerType;
            customer.Address = request.Address;
            customer.Rg = request.Rg;
            customer.BusinessName = request.BusinessName;
            customer.UserId = request.UserId;
            customer.UpdatedAt = DateTime.UtcNow;

            context.Customers.Update(customer);
            await context.SaveChangesAsync();

            return new Response<Customer?>(customer, message: 
                "Cliente atualizado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Customer?>(null, 500, 
                $"Não foi possível atualizar o cliente\n{ex.Message}");
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
                return new Response<Customer?>(null, 404, 
                    "Cliente não encontrado");

            customer.IsActive = false;

            await context.SaveChangesAsync();

            return new Response<Customer?>(customer, message: 
                "Cliente excluído com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Customer?>(null, 500, 
                $"Não foi possível excluir o cliente\n{ex.Message}");
        }
    }

    public async Task<Response<Customer?>> GetByIdAsync(GetCustomerByIdRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id 
                                          && c.UserId == request.UserId
                                          &&  c.IsActive);

            return customer is null
                ? new Response<Customer?>(null, 404, 
                    "Cliente não encontrado")
                : new Response<Customer?>(customer);
        }
        catch (Exception ex)
        {
            return new Response<Customer?>(null, 500, 
                $"Não foi possível retornar o cliente\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Customer>?>> GetAllAsync(GetAllCustomersRequest request)
    {
        try
        {
            var query = context
                .Customers
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId && c.IsActive);

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
            return new PagedResponse<List<Customer>?>(null, 500, 
                "Não foi possível consultar os clientes");
        }
    }
}