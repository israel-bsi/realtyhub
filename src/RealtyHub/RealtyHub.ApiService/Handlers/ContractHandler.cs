using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ContractHandler(AppDbContext context) : IContractHandler
{
    public async Task<Response<Contract?>> CreateAsync(CreateContractRequest request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o=>o.Customer)
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

            if (offer is null)
                return new Response<Contract?>(null, 404, 
                    "Proposta não encontrada");

            if (offer.OfferStatus != EOfferStatus.Accepted)
                return new Response<Contract?>(null, 400,
                    "A proposta precisa estar aceita para criar um contrato");

            context.Attach(offer);

            var contract = new Contract
            {
                IssueDate = request.IssueDate,
                SignatureDate = request.SignatureDate,
                EffectiveDate = request.EffectiveDate,
                TermEndDate = request.TermEndDate,
                Content = $"Contrato do cliente {offer.Customer.Name}\n" +
                                    $"Documento: {offer.Customer.DocumentNumber}\n" +
                                    $"Tipo Documento: {offer.Customer.DocumentType.ToString()}\n" +
                                    $"Telefone: {offer.Customer.Phone}\n" +
                                    $"Email: {offer.Customer.Email}\n" +
                                    $"Valor: {offer.Amount}\n" +
                                    $"Data de emissão: {request.IssueDate}\n" +
                                    $"Data de assinatura: {request.SignatureDate}\n" +
                                    $"Data de vigência: {request.EffectiveDate}\n" +
                                    $"Data de término: {request.TermEndDate}\n" +
                                    $"Imovel: {offer.Property.Title}\n" +
                                    $"Endereço: {offer.Property.Address.Neighborhood}\n",
                Offer = offer,
                OfferId = request.OfferId,
                IsActive = true
            };

            await context.Contracts.AddAsync(contract);
            await context.SaveChangesAsync();

            return new Response<Contract?>(contract, 201, 
                "Contrato criado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Contract?>(null, 500, 
                $"Não foi possível criar o contrato\n{ex.Message}");
        }
    }

    public async Task<Response<Contract?>> UpdateAsync(UpdateContractRequest request)
    {
        try
        {
            var contract = await context
                .Contracts
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404,
                    "Contrato não encontrado");

            var offer = await context
                .Offers
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

            if (offer is null)
                return new Response<Contract?>(null, 404,
                    "Proposta não encontrada");

            contract.IssueDate = request.IssueDate;
            contract.SignatureDate = request.SignatureDate;
            contract.EffectiveDate = request.EffectiveDate;
            contract.TermEndDate = request.TermEndDate;
            contract.OfferId = request.OfferId;
            contract.IsActive = request.IsActive;
            contract.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Contract?>(contract, 200,
                "Contrato atualizado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Contract?>(null, 500,
                $"Não foi possível atualizar o contrato\n{ex.Message}");
        }
    }

    public async Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request)
    {
        try
        {
            var contract = await context
                .Contracts
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404,
                    "Contrato não encontrado");

            contract.IsActive = false;
            await context.SaveChangesAsync();

            return new Response<Contract?>(contract, 200,
                "Contrato excluído com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Contract?>(null, 500,
                $"Não foi possível excluir o contrato\n{ex.Message}");
        }
    }

    public async Task<Response<Contract?>> GetByIdAsync(GetContractByIdRequest request)
    {
        try
        {
            var contract = await context
                .Contracts
                .AsNoTracking()
                .Include(c => c.Offer)
                .ThenInclude(o => o.Customer)
                .Include(c => c.Offer.Property)
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404,
                    "Contrato não encontrado");

            return new Response<Contract?>(contract, 200,
                "Contrato encontrado com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Contract?>(null, 500,
                $"Não foi possível encontrar o contrato\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Contract>?>> GetAllAsync(GetAllContractsRequest request)
    {
        try
        {
            var query = context
                .Contracts
                .AsNoTracking()
                .Include(c => c.Offer)
                .ThenInclude(o => o.Customer)
                .Include(c => c.Offer.Property)
                .Where(c => c.IsActive);

            var contracts = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Contract>?>(
                contracts, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Contract>?>(null, 500,
                $"Não foi possível retornar os contratos\n{ex.Message}");
        }
    }
}