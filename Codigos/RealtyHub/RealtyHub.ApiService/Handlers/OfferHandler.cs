﻿using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas a propostas.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IOfferHandler"/> e fornece
/// métodos para criar, atualizar, rejeitar, aceitar e buscar propostas no banco de dados.
/// </remarks>
public class OfferHandler : IOfferHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com propostas.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nas propostas.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="OfferHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nas propostas.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com propostas.</param>
    public OfferHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria uma nova proposta no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método adiciona uma nova proposta à base de dados e salva as alterações.
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para criação da proposta.</param>
    /// <returns>Retorna uma resposta com a proposta criada ou um erro.</returns>
    public async Task<Response<Offer?>> CreateAsync(Offer request)
    {
        Property? property;
        try
        {
            property = await _context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && p.IsActive);

            if (property is null)
                return new Response<Offer?>(null, 404, "Imóvel não encontrado");

            _context.Attach(property);
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Falha ao obter imóvel");
        }

        try
        {
            var payments = request.Payments.Select(paymentRequest => new Payment
            {
                Amount = paymentRequest.Amount,
                PaymentType = paymentRequest.PaymentType,
                Installments = paymentRequest.Installments,
                InstallmentsCount = paymentRequest.InstallmentsCount,
                UserId = request.UserId,
                IsActive = true
            }).ToList();

            var total = payments.Sum(p => p.Amount);
            if (total < request.Amount)
                return new Response<Offer?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            request.Buyer!.IsActive = true;
            var offer = new Offer
            {
                SubmissionDate = request.SubmissionDate,
                Amount = request.Amount,
                OfferStatus = request.OfferStatus,
                BuyerId = request.BuyerId,
                Buyer = request.Buyer,
                Payments = payments,
                PropertyId = request.PropertyId,
                Property = property,
                UserId = request.UserId
            };

            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();

            return new Response<Offer?>(offer, 201, "Proposta criada com sucesso");
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível criar a proposta");
        }
    }

    /// <summary>
    /// Atualiza uma proposta existente no banco de dados.
    /// </summary>
    /// <remarks>
    /// <para>Este método atualiza as informações de uma proposta existente no banco de dados.</para> 
    /// <para>O método verifica se a proposta existe e se o status da proposta permite a atualização.</para> 
    /// <para>Além disso, ele valida se o valor total dos pagamentos corresponde ao valor da proposta.</para> 
    /// <para>Se a proposta for encontrada e o status permitir, as informações da proposta são atualizadas.</para> 
    /// </remarks>
    /// <param name="request">Objeto contendo as novas informações da proposta.</param>
    /// <returns>Retorna a resposta com a proposta atualizada ou um erro se não for encontrada.</returns>
    public async Task<Response<Offer?>> UpdateAsync(Offer request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Offer?>(null, 404,
                    "Proposta não encontrada");

            if (offer.OfferStatus is EOfferStatus.Accepted or EOfferStatus.Rejected)
                return new Response<Offer?>(null, 400,
                    "Não é possível atualizar uma proposta aceita ou rejeitada");

            var total = request.Payments.Sum(p => p.Amount);
            if (total < request.Amount)
                return new Response<Offer?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            var payments = await _context
                .Payments
                .Where(p => p.OfferId == request.Id
                            && p.UserId == request.UserId)
                .ToListAsync();

            offer.SubmissionDate = request.SubmissionDate;
            offer.Amount = request.Amount;
            offer.PropertyId = request.PropertyId;
            offer.BuyerId = request.BuyerId;
            offer.OfferStatus = request.OfferStatus;
            offer.UpdatedAt = DateTime.UtcNow;
            foreach (var payment in payments)
            {
                var updatePaymentRequest = request.Payments
                    .FirstOrDefault(p => p.Id == payment.Id);

                if (updatePaymentRequest is not null)
                {
                    payment.Amount = updatePaymentRequest.Amount;
                    payment.PaymentType = updatePaymentRequest.PaymentType;
                    payment.Installments = updatePaymentRequest.Installments;
                    payment.InstallmentsCount = updatePaymentRequest.InstallmentsCount;
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.IsActive = true;
                }
                else
                {
                    payment.IsActive = false;
                    payment.UpdatedAt = DateTime.UtcNow;
                }
            }

            foreach (var payment in request.Payments)
            {
                var isExist = payments.Any(p => p.Id == payment.Id);
                if (isExist) continue;

                var newPayment = new Payment
                {
                    Amount = payment.Amount,
                    PaymentType = payment.PaymentType,
                    Installments = payment.Installments,
                    InstallmentsCount = payment.InstallmentsCount,
                    IsActive = true,
                    OfferId = offer.Id,
                    Offer = offer,
                    UserId = request.UserId
                };

                _context.Attach(newPayment);
                offer.Payments.Add(newPayment);
            }

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200, "Proposta atualizada com sucesso");
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível atualizar a proposta");
        }
    }

    /// <summary>
    /// Rejeita uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método rejeita uma proposta existente no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da proposta a ser rejeitada.</param>
    /// <returns>Retorna a resposta com a proposta rejeitada ou um erro se não for encontrada.</returns>
    public async Task<Response<Offer?>> RejectAsync(RejectOfferRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
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

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200, "Proposta rejeitada com sucesso");
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível rejeitar a proposta");
        }
    }

    /// <summary>
    /// Aceita uma proposta existente.
    /// </summary>
    /// <remarks>
    /// Este método aceita uma proposta existente no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da proposta a ser aceita.</param>
    /// <returns>Retorna a resposta com a proposta aceita ou um erro se não for encontrada.</returns>
    public async Task<Response<Offer?>> AcceptAsync(AcceptOfferRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
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

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Offer?>(offer, 200, "Proposta aceita com sucesso");
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível aceitar a proposta");
        }
    }

    /// <summary>
    /// Obtém uma proposta específica pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método busca uma proposta no banco de dados com base no ID fornecido.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da proposta desejada.</param>
    /// <returns>Retorna a proposta ou um erro caso não seja encontrada.</returns>
    public async Task<Response<Offer?>> GetByIdAsync(GetOfferByIdRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                    .ThenInclude(p => p!.Seller)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Offer?>(null, 404, "Proposta não encontrada");

            offer.SubmissionDate = offer.SubmissionDate?.ToLocalTime();

            return new Response<Offer?>(offer);
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível buscar a proposta");
        }
    }

    /// <summary>
    /// Obtém a proposta aceita para um imóvel específico.
    /// </summary>
    /// <remarks>
    /// Este método busca a proposta aceita para um imóvel específico no banco de dados.
    /// </remarks>
    /// <param name="request">Requisição contendo o ID do imóvel.</param>
    /// <returns>Retorna a proposta aceita ou um erro caso não seja encontrada.</returns>
    public async Task<Response<Offer?>> GetAcceptedByProperty(GetOfferAcceptedByProperty request)
    {
        try
        {
            var offer = await _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                    .ThenInclude(p => p!.Seller)
                .Include(o => o.Contract)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.PropertyId == request.PropertyId
                                          && o.OfferStatus == EOfferStatus.Accepted);

            if (offer is null)
                return new Response<Offer?>(null, 404, "Proposta aceita não encontrada");

            offer.SubmissionDate = offer.SubmissionDate?.ToLocalTime();

            return new Response<Offer?>(offer, 200, "Proposta aceita encontrada");
        }
        catch
        {
            return new Response<Offer?>(null, 500, "Não foi possível buscar a proposta aceita");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as propostas de um imóvel específico.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todas as propostas de um imóvel específico no banco de dados.</para>
    /// <para>Ele aplica filtros de data e paginação, retornando uma lista de propostas.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo parâmetros de paginação e filtro por imóvel.</param>
    /// <returns>Retorna uma resposta paginada com as propostas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByPropertyAsync(
        GetAllOffersByPropertyRequest request)
    {
        try
        {
            var property = await _context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && p.IsActive);

            if (property is null)
                return new PagedResponse<List<Offer>?>(null, 404,
                    "Imóvel não encontrado");

            var query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                    .ThenInclude(p => p!.Seller)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
                .Where(o => o.PropertyId == request.PropertyId);

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

            foreach (var offer in offers)
            {
                offer.SubmissionDate = offer.SubmissionDate?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Offer>?>(null, 500, "Não foi possível buscar as propostas");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as propostas de um cliente específico.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todas as propostas de um cliente específico no banco de dados.</para>
    /// <para>Ele aplica filtros de data e paginação, retornando uma lista de propostas.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo parâmetros de paginação e filtro por cliente.</param>
    /// <returns>Retorna uma resposta paginada com as propostas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Offer>?>> GetAllOffersByCustomerAsync(
       GetAllOffersByCustomerRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.CustomerId
                                          && p.IsActive);

            if (customer is null)
                return new PagedResponse<List<Offer>?>(null, 404,
                    "Cliente não encontrado");

            var query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                    .ThenInclude(p => p!.Seller)
                .Include(o => o.Payments)
                .Include(o => o.Contract)
                .Where(o => o.BuyerId == request.CustomerId
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

            foreach (var offer in offers)
            {
                offer.SubmissionDate = offer.SubmissionDate?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Offer>?>(null, 500, "Não foi possível buscar as propostas");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as propostas no sistema.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todas as propostas no banco de dados.</para>
    /// <para>Ele aplica filtros de data e paginação, retornando uma lista de propostas.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com as propostas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Offer>?>> GetAllAsync(GetAllOffersRequest request)
    {
        try
        {
            IQueryable<Offer> query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .ThenInclude(p => p!.Seller)
                .Include(o => o.Payments)
                .Include(o => o.Contract);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.SubmissionDate >= startDate
                                         && o.SubmissionDate <= endDate);
            }

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var offer in offers)
            {
                offer.SubmissionDate = offer.SubmissionDate?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Offer>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Offer>?>(null, 500, "Não foi possível buscar as propostas");
        }
    }
}