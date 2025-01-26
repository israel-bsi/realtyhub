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
    public async Task<Response<Contract?>> CreateAsync(Contract request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .ThenInclude(o=>o.Seller)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId
                                          && o.UserId == request.UserId);

            if (offer is null)
                return new Response<Contract?>(null, 404, "Proposta não encontrada");

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
                Content = $"Contrato do cliente {offer.Buyer.Name}\n" +
                                    $"Documento: {offer.Buyer.DocumentNumber}\n" +
                                    $"Tipo Cliente: {offer.Buyer.PersonType}\n" +
                                    $"Telefone: {offer.Buyer.Phone}\n" +
                                    $"Email: {offer.Buyer.Email}\n" +
                                    $"Valor: {offer.Amount}\n" +
                                    $"Data de emissão: {request.IssueDate}\n" +
                                    $"Data de assinatura: {request.SignatureDate}\n" +
                                    $"Data de vigência: {request.EffectiveDate}\n" +
                                    $"Data de término: {request.TermEndDate}\n" +
                                    $"Imovel: {offer.Property.Title}\n" +
                                    $"Endereço: {offer.Property.Address.Neighborhood}\n",
                Buyer = offer.Buyer,
                BuyerId = offer.Buyer.Id,
                SellerId = offer.Property.SellerId,
                Seller = offer.Property.Seller,
                Offer = offer,
                OfferId = offer.Id,
                IsActive = true,
                UserId = request.UserId
            };

            await context.Contracts.AddAsync(contract);
            await context.SaveChangesAsync();

            return new Response<Contract?>(contract, 201, "Contrato criado com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível criar o contrato");
        }
    }

    public async Task<Response<Contract?>> UpdateAsync(Contract request)
    {
        try
        {
            var contract = await context
                .Contracts
                .Include(o => o.Seller)
                .Include(o => o.Buyer)
                .Include(o => o.Offer)
                .ThenInclude(o => o.Property)
                .FirstOrDefaultAsync(c => c.Id == request.Id 
                                          && c.UserId == request.UserId 
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            var offer = await context
                .Offers
                .Include(offer => offer.Property)
                .ThenInclude(property => property.Seller)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId 
                                          && o.UserId == request.UserId);

            if (offer is null)
                return new Response<Contract?>(null, 404, "Proposta não encontrada");

            contract.IssueDate = request.IssueDate;
            contract.SignatureDate = request.SignatureDate;
            contract.EffectiveDate = request.EffectiveDate;
            contract.TermEndDate = request.TermEndDate;
            contract.Buyer = request.Buyer;
            contract.BuyerId = request.BuyerId;
            contract.SellerId = offer.Property.SellerId;
            contract.Seller = offer.Property.Seller;
            contract.Offer = offer;
            contract.OfferId = request.OfferId;
            contract.IsActive = request.IsActive;
            contract.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new Response<Contract?>(contract, 200, "Contrato atualizado com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível atualizar o contrato");
        }
    }

    public async Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request)
    {
        try
        {
            var contract = await context
                .Contracts
                .FirstOrDefaultAsync(c => c.Id == request.Id 
                                          && c.UserId == request.UserId 
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            contract.IsActive = false;
            contract.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return new Response<Contract?>(null, 204, "Contrato excluído com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível excluir o contrato");
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
                .ThenInclude(o => o.Buyer)
                .Include(c => c.Offer.Property)
                .ThenInclude(p => p.Seller)
                .FirstOrDefaultAsync(c => c.Id == request.Id 
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            return contract is null 
                ? new Response<Contract?>(null, 404, "Contrato não encontrado") 
                : new Response<Contract?>(contract);
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível encontrar o contrato");
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
                .ThenInclude(o => o.Buyer)
                .Include(c => c.Offer.Property)
                .ThenInclude(p => p.Seller)
                .Where(c => c.UserId == request.UserId && c.IsActive);

            var contracts = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Contract>?>(
               contracts, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Contract>?>(null, 500, 
                "Não foi possível retornar os contratos");
        }
    }
}