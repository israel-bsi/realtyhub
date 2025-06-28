using Microsoft.EntityFrameworkCore;
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
    public async Task<Response<Proposta?>> CreateAsync(Proposta request)
    {
        Imovel? property;
        try
        {
            property = await _context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ImovelId
                                          && p.Ativo);

            if (property is null)
                return new Response<Proposta?>(null, 404, "Imóvel não encontrado");

            _context.Attach(property);
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Falha ao obter imóvel");
        }

        try
        {
            var payments = request.Pagamentos.Select(paymentRequest => new Pagamento
            {
                Valor = paymentRequest.Valor,
                TipoPagamento = paymentRequest.TipoPagamento,
                Parcelado = paymentRequest.Parcelado,
                QuantidadeParcelas = paymentRequest.QuantidadeParcelas,
                UsuarioId = request.UsuarioId,
                Ativo = true
            }).ToList();

            var total = payments.Sum(p => p.Valor);
            if (total < request.Valor)
                return new Response<Proposta?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            request.Comprador!.Ativo = true;
            var offer = new Proposta
            {
                DataDeEnvio = request.DataDeEnvio,
                Valor = request.Valor,
                StatusProposta = request.StatusProposta,
                CompradorId = request.CompradorId,
                Comprador = request.Comprador,
                Pagamentos = payments,
                ImovelId = request.ImovelId,
                Imovel = property,
                UsuarioId = request.UsuarioId
            };

            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();

            return new Response<Proposta?>(offer, 201, "Proposta criada com sucesso");
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível criar a proposta");
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
    public async Task<Response<Proposta?>> UpdateAsync(Proposta request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Proposta?>(null, 404,
                    "Proposta não encontrada");

            if (offer.StatusProposta is EStatusProposta.Aceita or EStatusProposta.Rejeitada)
                return new Response<Proposta?>(null, 400,
                    "Não é possível atualizar uma proposta aceita ou rejeitada");

            var total = request.Pagamentos.Sum(p => p.Valor);
            if (total < request.Valor)
                return new Response<Proposta?>(null, 400,
                    "O valor total dos pagamentos não corresponde ao valor da proposta");

            var payments = await _context
                .Payments
                .Where(p => p.PropostaId == request.Id
                            && p.UsuarioId == request.UsuarioId)
                .ToListAsync();

            offer.DataDeEnvio = request.DataDeEnvio;
            offer.Valor = request.Valor;
            offer.ImovelId = request.ImovelId;
            offer.CompradorId = request.CompradorId;
            offer.StatusProposta = request.StatusProposta;
            offer.AtualizadoEm = DateTime.UtcNow;
            foreach (var payment in payments)
            {
                var updatePaymentRequest = request.Pagamentos
                    .FirstOrDefault(p => p.Id == payment.Id);

                if (updatePaymentRequest is not null)
                {
                    payment.Valor = updatePaymentRequest.Valor;
                    payment.TipoPagamento = updatePaymentRequest.TipoPagamento;
                    payment.Parcelado = updatePaymentRequest.Parcelado;
                    payment.QuantidadeParcelas = updatePaymentRequest.QuantidadeParcelas;
                    payment.AtualizadoEm = DateTime.UtcNow;
                    payment.Ativo = true;
                }
                else
                {
                    payment.Ativo = false;
                    payment.AtualizadoEm = DateTime.UtcNow;
                }
            }

            foreach (var payment in request.Pagamentos)
            {
                var isExist = payments.Any(p => p.Id == payment.Id);
                if (isExist) continue;

                var newPayment = new Pagamento
                {
                    Valor = payment.Valor,
                    TipoPagamento = payment.TipoPagamento,
                    Parcelado = payment.Parcelado,
                    QuantidadeParcelas = payment.QuantidadeParcelas,
                    Ativo = true,
                    PropostaId = offer.Id,
                    Proposta = offer,
                    UsuarioId = request.UsuarioId
                };

                _context.Attach(newPayment);
                offer.Pagamentos.Add(newPayment);
            }

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Proposta?>(offer, 200, "Proposta atualizada com sucesso");
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível atualizar a proposta");
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
    public async Task<Response<Proposta?>> RejectAsync(RejectOfferRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Proposta?>(null, 404,
                    "Proposta não encontrada");

            switch (offer.StatusProposta)
            {
                case EStatusProposta.Aceita:
                    return new Response<Proposta?>(null, 400,
                        "Não é possível rejeitar uma proposta aceita");
                case EStatusProposta.Rejeitada:
                    return new Response<Proposta?>(null, 400,
                        "Proposta já está recusada");
            }

            offer.StatusProposta = EStatusProposta.Rejeitada;
            offer.AtualizadoEm = DateTime.UtcNow;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Proposta?>(offer, 200, "Proposta rejeitada com sucesso");
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível rejeitar a proposta");
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
    public async Task<Response<Proposta?>> AcceptAsync(AcceptOfferRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Proposta?>(null, 404,
                    "Proposta não encontrada");

            switch (offer.StatusProposta)
            {
                case EStatusProposta.Aceita:
                    return new Response<Proposta?>(null, 400,
                        "Proposta já está aceita");
                case EStatusProposta.Rejeitada:
                    return new Response<Proposta?>(null, 400,
                        "Não é possível aceitar uma proposta recusada");
            }

            offer.StatusProposta = EStatusProposta.Aceita;
            offer.AtualizadoEm = DateTime.UtcNow;

            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();

            return new Response<Proposta?>(offer, 200, "Proposta aceita com sucesso");
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível aceitar a proposta");
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
    public async Task<Response<Proposta?>> GetByIdAsync(GetOfferByIdRequest request)
    {
        try
        {
            var offer = await _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                    .ThenInclude(p => p!.Vendedor)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .FirstOrDefaultAsync(o => o.Id == request.Id);

            if (offer is null)
                return new Response<Proposta?>(null, 404, "Proposta não encontrada");

            offer.DataDeEnvio = offer.DataDeEnvio?.ToLocalTime();

            return new Response<Proposta?>(offer);
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível buscar a proposta");
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
    public async Task<Response<Proposta?>> GetAcceptedByProperty(GetOfferAcceptedByProperty request)
    {
        try
        {
            var offer = await _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                    .ThenInclude(p => p!.Vendedor)
                .Include(o => o.Contrato)
                .Include(o => o.Pagamentos)
                .FirstOrDefaultAsync(o => o.ImovelId == request.PropertyId
                                          && o.StatusProposta == EStatusProposta.Aceita);

            if (offer is null)
                return new Response<Proposta?>(null, 404, "Proposta aceita não encontrada");

            offer.DataDeEnvio = offer.DataDeEnvio?.ToLocalTime();

            return new Response<Proposta?>(offer, 200, "Proposta aceita encontrada");
        }
        catch
        {
            return new Response<Proposta?>(null, 500, "Não foi possível buscar a proposta aceita");
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
    public async Task<PagedResponse<List<Proposta>?>> GetAllOffersByPropertyAsync(
        GetAllOffersByPropertyRequest request)
    {
        try
        {
            var property = await _context
                .Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId
                                          && p.Ativo);

            if (property is null)
                return new PagedResponse<List<Proposta>?>(null, 404,
                    "Imóvel não encontrado");

            var query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                    .ThenInclude(p => p!.Vendedor)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .Where(o => o.ImovelId == request.PropertyId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.DataDeEnvio >= startDate
                                         && o.DataDeEnvio <= endDate);
            }

            query = query.OrderBy(o => o.DataDeEnvio);

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var offer in offers)
            {
                offer.DataDeEnvio = offer.DataDeEnvio?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Proposta>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Proposta>?>(null, 500, "Não foi possível buscar as propostas");
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
    public async Task<PagedResponse<List<Proposta>?>> GetAllOffersByCustomerAsync(
       GetAllOffersByCustomerRequest request)
    {
        try
        {
            var customer = await _context
                .Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.CustomerId
                                          && p.Ativo);

            if (customer is null)
                return new PagedResponse<List<Proposta>?>(null, 404,
                    "Cliente não encontrado");

            var query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                    .ThenInclude(p => p!.Vendedor)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato)
                .Where(o => o.CompradorId == request.CustomerId
                            && o.UsuarioId == request.UserId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.DataDeEnvio >= startDate
                                         && o.DataDeEnvio <= endDate);
            }

            query = query.OrderBy(o => o.DataDeEnvio);

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var offer in offers)
            {
                offer.DataDeEnvio = offer.DataDeEnvio?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Proposta>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Proposta>?>(null, 500, "Não foi possível buscar as propostas");
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
    public async Task<PagedResponse<List<Proposta>?>> GetAllAsync(GetAllOffersRequest request)
    {
        try
        {
            IQueryable<Proposta> query = _context
                .Offers
                .AsNoTracking()
                .Include(o => o.Comprador)
                .Include(o => o.Imovel)
                .ThenInclude(p => p!.Vendedor)
                .Include(o => o.Pagamentos)
                .Include(o => o.Contrato);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(o => o.DataDeEnvio >= startDate
                                         && o.DataDeEnvio <= endDate);
            }

            var offers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var offer in offers)
            {
                offer.DataDeEnvio = offer.DataDeEnvio?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Proposta>?>(offers, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Proposta>?>(null, 500, "Não foi possível buscar as propostas");
        }
    }
}