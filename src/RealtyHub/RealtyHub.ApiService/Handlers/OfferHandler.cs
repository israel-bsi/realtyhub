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
    public async Task<Response<Offer?>> CreateAsync(CreateOfferRequest request)
    {
        Customer? customer;
        try
        {
            customer = await context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.IsActive);

            if (customer is null)
                return new Response<Offer?>(null, 404, 
                    "Cliente não encontrado");

            context.Attach(customer);
        }
        catch (Exception ex)
        {
            return new Response<Offer?>(null, 500,
                $"Falha ao obter cliente\n{ex.Message}");
        }

        Property? property;
        try
        {
            property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.IsActive);

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
            var payments = new List<Payment>();
            foreach (var paymentRequest in request.CreatePaymentRequests)
            {
                var payment = new Payment
                {
                    PaymentDate = paymentRequest.PaymentDate,
                    Amount = paymentRequest.Amount,
                    PaymentType = paymentRequest.PaymentType,
                    PaymentStatus = paymentRequest.PaymentStatus,
                    IsActive = true
                };
                payments.Add(payment);
            }

            var total = payments.Sum(p => p.Amount);
            if (total < request.Amount)
                return new Response<Offer?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            var offer = new Offer
            {
                Submission = request.Submission,
                Amount = request.Amount,
                OfferStatus = request.OfferStatus,
                CustomerId = request.CustomerId,
                Customer = customer,
                PropertyId = request.PropertyId,
                Property = property,
                Payments = payments
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

    public async Task<Response<Offer?>> UpdateAsync(UpdateOfferRequest request)
    {
        try
        {
            var offer = await context
                .Offers
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Offer?>(null, 404, 
                    "Proposta não encontrada");

            if (offer.OfferStatus is EOfferStatus.Accepted or EOfferStatus.Rejected)
                return new Response<Offer?>(null, 400,
                    "Não é possível atualizar uma proposta aceita ou rejeitada");

            var total = request.UpdatePaymentRequests.Sum(p => p.Amount);
            if (total < request.Amount)
                return new Response<Offer?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            offer.Submission = request.Submission;
            offer.Amount = request.Amount;
            offer.PropertyId = request.PropertyId;
            offer.CustomerId = request.CustomerId;
            offer.OfferStatus = request.OfferStatus;
            offer.Payments = request.UpdatePaymentRequests.Select(p => new Payment
            {
                Id = p.Id,
                PaymentDate = p.PaymentDate,
                Amount = p.Amount,
                PaymentType = p.PaymentType,
                PaymentStatus = p.PaymentStatus,
                IsActive = true
            }).ToList();
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
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

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
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

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
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

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

    public async Task<Response<List<Offer>?>> GetAllOffersByPropertyAsync(GetAllOffersByPropertyRequest request)
    {
        try
        {
            var property = await context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.IsActive);

            if (property is null)
                return new Response<List<Offer>?>(null, 404, 
                    "Imóvel não encontrado");

            var offers = await context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .Where(o => o.PropertyId == request.PropertyId)
                .ToListAsync();

            var count = offers.Count;

            return offers.Count == 0
                ? new Response<List<Offer>?>(null, 404, 
                    "Nenhuma proposta encontrada para esse imóvel")
                : new Response<List<Offer>?>(offers, 200, 
                    $"{count} proposta(s) encontradas");
        }
        catch (Exception ex)
        {
            return new Response<List<Offer>?>(null, 500,
                $"Não foi possível buscar as propostas\n{ex.Message}");
        }
    }

    public async Task<Response<List<Offer>?>> GetAllOffersByCustomerAsync(GetAllOffersByCustomerRequest request)
    {
        try
        {
            var customer = await context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.CustomerId && p.IsActive);

            if (customer is null)
                return new Response<List<Offer>?>(null, 404, 
                    "Cliente não encontrado");

            var offers = await context
                .Offers
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .Where(o => o.CustomerId == request.CustomerId)
                .ToListAsync();

            var count = offers.Count;

            return offers.Count == 0
                ? new Response<List<Offer>?>(null, 404, 
                    "Nenhuma proposta feita por esse cliente foi encontrada")
                : new Response<List<Offer>?>(offers, 200, 
                    $"{count} proposta(s) encontradas");
        }
        catch (Exception ex)
        {
            return new Response<List<Offer>?>(null, 500,
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
                .Include(o => o.Payments);

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