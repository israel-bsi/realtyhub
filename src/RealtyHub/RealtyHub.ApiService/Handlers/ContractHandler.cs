using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ContractHandler(AppDbContext context, IHostEnvironment host) : IContractHandler
{
    public async Task<Response<Contract?>> CreateAsync(Contract request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .ThenInclude(o => o!.Seller)
                .Include(o=>o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

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
                FileId = Guid.NewGuid().ToString(),
                Buyer = offer.Buyer,
                BuyerId = offer.Buyer!.Id,
                SellerId = offer.Property!.SellerId,
                Seller = offer.Property.Seller,
                Offer = offer,
                OfferId = offer.Id,
                IsActive = true,
                UserId = request.UserId
            };

            await CreateOrUpdateContract(contract, false);

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
                .ThenInclude(o => o!.Property)
                .FirstOrDefaultAsync(c => c.Id == request.Id 
                                          && c.UserId == request.UserId 
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            var offer = await context
                .Offers
                .Include(offer => offer.Property)
                .ThenInclude(property => property!.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

            if (offer is null)
                return new Response<Contract?>(null, 404, "Proposta não encontrada");

            contract.IssueDate = request.IssueDate;
            contract.SignatureDate = request.SignatureDate;
            contract.EffectiveDate = request.EffectiveDate;
            contract.TermEndDate = request.TermEndDate;
            contract.Buyer = request.Buyer;
            contract.BuyerId = request.BuyerId;
            contract.SellerId = offer.Property!.SellerId;
            contract.Seller = offer.Property.Seller;
            contract.Offer = offer;
            contract.OfferId = request.OfferId;
            contract.FileId = request.FileId;
            contract.IsActive = request.IsActive;
            contract.UpdatedAt = DateTime.UtcNow;

            await CreateOrUpdateContract(contract, true);

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
                .ThenInclude(o=>o!.Property)
                .Include(c=>c.Buyer)
                .Include(c=>c.Seller)
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
                .ThenInclude(o => o!.Property)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
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

    private async Task CreateOrUpdateContract(Contract contract, bool update)
    {
        var contractModelType = contract.Buyer?.PersonType switch
        {
            EPersonType.Business when contract.Seller?.PersonType == EPersonType.Business 
                => EContractModelType.PJPJ,
            EPersonType.Individual when contract.Seller?.PersonType == EPersonType.Individual 
                => EContractModelType.PFPF,
            EPersonType.Business when contract.Seller?.PersonType == EPersonType.Individual 
                => EContractModelType.PFPJ,
            _ => EContractModelType.PJPF
        };

        var contractModel = await context
            .ContractTemplates
            .FirstOrDefaultAsync(cm => cm.Type == contractModelType);

        var docxContractGenerator = new ContractGenerator();
        var pathContracts = Path.Combine(host.ContentRootPath, "Sources", "Contracts");
        var pathContactFile = Path.Combine(pathContracts, $"{contract.Id}.docx");
        if (update)
            File.Delete(pathContactFile);
        docxContractGenerator.GenerateContract(pathContracts, contract, contractModel);
    }
}