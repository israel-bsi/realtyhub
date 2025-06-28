using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Viewings;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas às visitas de imóveis.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IViewingHandler"/> e fornece
/// métodos para agendar, reagendar, finalizar e cancelar visitas,
/// além de buscar visitas específicas ou todas as visitas.
/// </remarks>
public class ViewingHandler : IViewingHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com visitas.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nas visitas.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ViewingHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nas visitas.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com as visitas.</param>
    public ViewingHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Agenda uma nova visita para um imóvel.
    /// </summary>
    /// <remarks>
    /// <para>Este método adiciona uma nova visita à base de dados e salva as alterações. </para>
    /// <para>Ele verifica se o cliente e o imóvel existem e estão ativos antes de agendar a visita.</para>
    /// <para>Se a visita já estiver agendada para a mesma data, retorna um erro.</para>
    /// <para>O status da visita é definido como "Pendente" por padrão.</para>
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para agendamento da visita.</param>
    /// <returns>Retorna uma resposta com a visita agendada ou um erro em caso de falha.</returns>
    public async Task<Response<Visita?>> ScheduleAsync(Visita request)
    {
        try
        {
            var customer = await _context
                .Customers
                .FirstOrDefaultAsync(c => c.Id == request.CompradorId
                                          && (string.IsNullOrEmpty(c.UsuarioId) || c.UsuarioId == request.UsuarioId)
                                          && c.Ativo);

            if (customer is null)
                return new Response<Visita?>(null, 404, "Cliente não encontrado");

            var property = await _context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == request.ImovelId
                                          && (string.IsNullOrEmpty(p.UsuarioId) || p.UsuarioId == request.UsuarioId)
                                          && p.Ativo);

            if (property is null)
                return new Response<Visita?>(null, 404, "Imóvel não encontrado");

            var isViewingExist = await _context
                .Viewing
                .AnyAsync(v => v.ImovelId == request.ImovelId
                               && v.DataVisita == request.DataVisita);

            if (isViewingExist)
                return new Response<Visita?>(null, 400, "Visita já agendada para esta data");

            var viewing = new Visita
            {
                DataVisita = request.DataVisita,
                StatusVisita = request.StatusVisita,
                CompradorId = request.CompradorId,
                Comprador = customer,
                ImovelId = request.ImovelId,
                Imovel = property,
                UsuarioId = request.UsuarioId
            };

            _context.Viewing.Add(viewing);
            await _context.SaveChangesAsync();

            return new Response<Visita?>(viewing, 201, "Visita agendada com sucesso");
        }
        catch
        {
            return new Response<Visita?>(null, 500, "Não foi possível agendar a visita");
        }
    }

    /// <summary>
    /// Reagenda uma visita existente.
    /// </summary>
    /// <remarks>
    /// <para>Este método atualiza a data e o status de uma visita existente.</para>
    /// <para>Ele verifica se a visita existe e se não está cancelada ou finalizada.</para>
    /// <para>Se a visita não existir ou já estiver cancelada ou finalizada, retorna um erro.</para>
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para reagendamento da visita.</param>
    /// <returns>Retorna uma resposta com a visita reagendada ou um erro em caso de falha.</returns>
    public async Task<Response<Visita?>> RescheduleAsync(Visita request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UsuarioId) || v.UsuarioId == request.UsuarioId));

            if (viewing is null)
                return new Response<Visita?>(null, 404, "Visita não encontrada");

            switch (viewing.StatusVisita)
            {
                case EStatusVisita.Cancelada:
                    return new Response<Visita?>(null, 400, "Não é possível reagendar uma visita cancelada");
                case EStatusVisita.Finalizada:
                    return new Response<Visita?>(null, 400, "Não é possível reagendar uma visita finalizada");
            }

            viewing.DataVisita = request.DataVisita;
            viewing.AtualizadoEm = DateTime.UtcNow;

            _context.Viewing.Update(viewing);
            await _context.SaveChangesAsync();

            return new Response<Visita?>(viewing, message: "Visita reagendada com sucesso");
        }
        catch
        {
            return new Response<Visita?>(null, 500, "Não foi possível reagendar a visita");
        }
    }

    /// <summary>
    /// Finaliza uma visita existente.
    /// </summary>
    /// <remarks>
    /// <para>Este método atualiza o status de uma visita para "Finalizada".</para>
    /// <para>Ele verifica se a visita existe e se não está cancelada ou já finalizada.</para>
    /// <para>Se a visita não existir ou já estiver cancelada ou finalizada, retorna um erro.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da visita a ser finalizada.</param>
    /// <returns>Retorna uma resposta com a visita finalizada ou um erro em caso de falha.</returns>
    public async Task<Response<Visita?>> DoneAsync(DoneViewingRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UsuarioId) || v.UsuarioId == request.UserId));

            if (viewing is null)
                return new Response<Visita?>(null, 404, "Visita não encontrada");

            switch (viewing.StatusVisita)
            {
                case EStatusVisita.Cancelada:
                    return new Response<Visita?>(null, 400, "Não é possível finalizar uma visita cancelada");
                case EStatusVisita.Finalizada:
                    return new Response<Visita?>(null, 400, "Visita já finalizada");
            }

            viewing.StatusVisita = EStatusVisita.Finalizada;
            viewing.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Visita?>(viewing, message: "Visita finalizada com sucesso");
        }
        catch
        {
            return new Response<Visita?>(null, 500, "Não foi possível finalizar a visita");
        }
    }

    /// <summary>
    /// Cancela uma visita existente.
    /// </summary>
    /// <remarks>
    /// <para>Este método atualiza o status de uma visita para "Cancelada".</para>
    /// <para>Ele verifica se a visita existe e se não está finalizada.</para>
    /// <para>Se a visita não existir ou já estiver finalizada, retorna um erro.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da visita a ser cancelada.</param>
    /// <returns>Retorna uma resposta com a visita cancelada ou um erro em caso de falha.</returns>
    public async Task<Response<Visita?>> CancelAsync(CancelViewingRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UsuarioId) || v.UsuarioId == request.UserId));

            if (viewing is null)
                return new Response<Visita?>(null, 404, "Visita não encontrada");

            switch (viewing.StatusVisita)
            {
                case EStatusVisita.Finalizada:
                    return new Response<Visita?>(null, 400, "Não é possível cancelar uma visita finalizada");
                case EStatusVisita.Cancelada:
                    return new Response<Visita?>(null, 400, "Visita já cancelada");
            }

            viewing.StatusVisita = EStatusVisita.Cancelada;
            viewing.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new Response<Visita?>(viewing, message: "Visita cancelada com sucesso");
        }
        catch
        {
            return new Response<Visita?>(null, 500, "Não foi possível cancelar a visita");
        }
    }

    /// <summary>
    /// Obtém uma visita específica pelo ID.
    /// </summary>
    /// <remarks>
    /// <para>Este método busca uma visita no banco de dados com base no ID fornecido.</para>
    /// <para>Ele verifica se a visita existe e se não está cancelada ou finalizada.</para>
    /// <para>Se a visita não existir ou já estiver cancelada ou finalizada, retorna um erro.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo o ID da visita desejada.</param>
    /// <returns>Retorna a visita ou um erro caso não seja encontrada.</returns>
    public async Task<Response<Visita?>> GetByIdAsync(GetViewingByIdRequest request)
    {
        try
        {
            var viewing = await _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .ThenInclude(p => p!.Vendedor)
                .FirstOrDefaultAsync(v => v.Id == request.Id
                                          && (string.IsNullOrEmpty(v.UsuarioId) || v.UsuarioId == request.UserId));

            if (viewing is null)
                return new Response<Visita?>(null, 404, "Visita não encontrada");

            viewing.DataVisita = viewing.DataVisita?.ToLocalTime();
            return new Response<Visita?>(viewing);
        }
        catch
        {
            return new Response<Visita?>(null, 500, "Não foi possível retornar a visita");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todas as visitas.
    /// </summary>
    /// <remarks>
    /// <para>Este método busca todas as visitas no banco de dados e aplica filtros de data e paginação.</para>
    /// <para>Caso o filtro de data não seja fornecido, retorna todas as visitas.</para>
    /// </remarks>
    /// <param name="request">Requisição contendo parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com as visitas ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Visita>?>> GetAllAsync(GetAllViewingsRequest request)
    {
        try
        {
            var query = _context
                .Viewing
                .AsNoTracking()
                .Include(v => v.Comprador)
                .Include(v => v.Imovel)
                .ThenInclude(p => p!.Vendedor)
                .Where(v => string.IsNullOrEmpty(v.UsuarioId) || v.UsuarioId == request.UserId);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(v => v.DataVisita >= startDate
                                         && v.DataVisita <= endDate);
            }

            query = query.OrderBy(v => v.DataVisita);

            var viewings = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var viewing in viewings)
            {
                viewing.DataVisita = viewing.DataVisita?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Visita>?>(
                viewings, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Visita>?>(null, 500, "Não foi possível retornar as visitas");
        }
    }
}
