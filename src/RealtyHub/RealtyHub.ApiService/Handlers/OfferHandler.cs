using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class OfferHandler(AppDbContext context) : IOfferHandler
{
    public async Task<Response<Offer?>> CreateAsync(Offer request)
    {
        Property? property;
        try
        {
            property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && p.UserId == request.UserId
                                          && p.IsActive);

            if (property is null)
                return new Response<Offer?>(null, 404,
                    "Imóvel não encontrado");

            context.Attach(property);
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Falha ao obter imóvel\n{ex.Message}");
        }

        try
        {
            request.Customer.IsActive = true;
            var offer = new Offer
            {
                SubmissionDate = request.SubmissionDate,
                Amount = request.Amount,
                PaymentDetails = request.PaymentDetails,
                OfferStatus = request.OfferStatus,
                CustomerId = request.CustomerId,
                Customer = request.Customer,
                PropertyId = request.PropertyId,
                Property = property,
                UserId = request.UserId
            };

            await context.Offers.AddAsync(offer);
            await context.SaveChangesAsync();

            return new Response<Offer?>(offer, 201,
                "Proposta criada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Não foi possível criar a proposta\n{ex.Message}");
        }
    }

    public async Task<Response<Offer?>> UpdateAsync(Offer request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => o.Id == request.Id 
                                          && o.UserId == request.UserId);

            if (offer is null)
                return new Response<Offer?>(null, 404,
                    "Proposta não encontrada");

            if (offer.OfferStatus is EOfferStatus.Accepted or EOfferStatus.Rejected)
                return new Response<Offer?>(null, 400,
                    "Não é possível atualizar uma proposta aceita ou rejeitada");

            offer.SubmissionDate = request.SubmissionDate;
            offer.Amount = request.Amount;
            offer.PropertyId = request.PropertyId;
            offer.CustomerId = request.CustomerId;
            offer.OfferStatus = request.OfferStatus;
            offer.PaymentDetails = request.PaymentDetails;
            offer.UpdatedAt = DateTime.UtcNow;

            context.Offers.Update(offer);
            await context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200,
                "Proposta atualizada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Não foi possível atualizar a proposta\n{ex.Message}");
        }
    }

    public async Task<Response<Offer?>> RejectAsync(RejectOfferRequest request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => o.Id == request.Id 
                                          && o.UserId == request.UserId);

            if (offer is null)
                return new Response<Offer?>(null, 404,
                    "Proposta não encontrada");

            switch (offer.OfferStatus)
            {
                case EOfferStatus.Accepted:
                    return new Response<Offer?>(null, 400,
                        "Não é possível rejeitar uma proposta aceita");
                case EOfferStatus.Rejected:
                    return new Response<Offer?>(null, 400,
                        "Proposta já está recusada");
            }

            offer.OfferStatus = EOfferStatus.Rejected;
            offer.UpdatedAt = DateTime.UtcNow;

            context.Offers.Update(offer);
            await context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200,
                "Proposta rejeitada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Não foi possível rejeitar a proposta\n{ex.Message}");
        }
    }

    public async Task<Response<Offer?>> AcceptAsync(AcceptOfferRequest request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => o.Id == request.Id 
                                          && o.UserId == request.UserId);

            if (offer is null)
                return new Response<Offer?>(null, 404,
                    "Proposta não encontrada");

            switch (offer.OfferStatus)
            {
                case EOfferStatus.Accepted:
                    return new Response<Offer?>(null, 400,
                        "Proposta já está aceita");
                case EOfferStatus.Rejected:
                    return new Response<Offer?>(null, 400,
                        "Não é possível aceitar uma proposta recusada");
            }

            offer.OfferStatus = EOfferStatus.Accepted;
            offer.UpdatedAt = DateTime.UtcNow;

            context.Offers.Update(offer);
            await context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200,
                "Proposta aceita com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Não foi possível aceitar a proposta\n{ex.Message}");
        }
    }

    public async Task<Response<Offer?>> GetByIdAsync(GetOfferByIdRequest request)
    {
        try
        {
            var offer = await context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .FirstOrDefaultAsync(o => o.Id == request.Id 
                                          && o.UserId == request.UserId);

            return offer == null
                ? new Response<Offer?>(null, 404, "Proposta não encontrada")
                : new Response<Offer?>(offer, 200, "Proposta encontrada");
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Não foi possível buscar a proposta\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByPropertyAsync(GetAllOffersByPropertyRequest request)
    {
        try
        {
            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId 
                                          && p.UserId == request.UserId 
                                          && p.IsActive);

            if (property is null)
                return new PagedResponse<List<Offer>?>(null, 404,
                    "Imóvel não encontrado");

            var query = context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Where(o => o.PropertyId == request.PropertyId
                            && o.UserId == request.UserId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.SubmissionDate >= startDate
                                         && o.SubmissionDate <= endDate);
            }

            query = query.OrderBy(o => o.SubmissionDate);

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Offer>?>(null, 500,
                $"Não foi possível buscar as propostas\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByCustomerAsync(GetAllOffersByCustomerRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.CustomerId 
                                          && p.UserId == request.UserId 
                                          && p.IsActive);

            if (customer is null)
                return new PagedResponse<List<Offer>?>(null, 404,
                    "Cliente não encontrado");

            var query = context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Where(o => o.CustomerId == request.CustomerId
                            && o.UserId == request.UserId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.SubmissionDate >= startDate
                                         && o.SubmissionDate <= endDate);
            }

            query = query.OrderBy(o => o.SubmissionDate);

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Offer>?>(null, 500,
                $"Não foi possível buscar as propostas\n{ex.Message}");
        }
    }

    public async Task<PagedResponse<List<Offer>?>> GetAllAsync(GetAllOffersRequest request)
    {
        try
        {
            var query = context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Where(o => o.UserId == request.UserId);

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(
                offers, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Offer>?>(null, 500,
                $"Não foi possível buscar as propostas\n{ex.Message}");
        }
    }
}