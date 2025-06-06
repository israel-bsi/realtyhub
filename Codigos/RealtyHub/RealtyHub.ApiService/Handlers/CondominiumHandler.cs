﻿using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Condominiums;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Extensions;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas a condomínios.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="ICondominiumHandler"/> e fornece
/// métodos para criar, atualizar, deletar e buscar condomínios no banco de dados.
/// </remarks>
public class CondominiumHandler : ICondominiumHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com condomínios.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nos condomínios.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CondominiumHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nos condomínios.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com condomínios.</param>
    public CondominiumHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo condomínio no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método adiciona um novo condomínio à base de dados e salva as alterações.
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para criação do condomínio.</param>
    /// <returns>Retorna uma resposta com o condomínio criado e o código de status.</returns>
    public async Task<Response<Condominium?>> CreateAsync(Condominium request)
    {
        try
        {
            var condominium = new Condominium
            {
                Name = request.Name,
                Address = request.Address,
                Units = request.Units,
                Floors = request.Floors,
                HasElevator = request.HasElevator,
                HasSwimmingPool = request.HasSwimmingPool,
                HasPartyRoom = request.HasPartyRoom,
                HasPlayground = request.HasPlayground,
                HasFitnessRoom = request.HasFitnessRoom,
                CondominiumValue = request.CondominiumValue,
                IsActive = true
            };

            await _context.Condominiums.AddAsync(condominium);
            await _context.SaveChangesAsync();

            return new Response<Condominium?>(condominium, 201);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível criar o condomínio");
        }
    }

    /// <summary>
    /// Atualiza um condomínio existente.
    /// </summary>
    /// <remarks>
    /// Este método atualiza as informações de um condomínio existente no banco de dados.
    /// </remarks>
    /// <param name="request">Objeto contendo as novas informações do condomínio.</param>
    /// <returns>Retorna a resposta com o condomínio atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Condominium?>> UpdateAsync(Condominium request)
    {
        try
        {
            var condominium = await _context
                .Condominiums
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (condominium is null)
                return new Response<Condominium?>(null, 404, "Condomínio não encontrado");

            condominium.Name = request.Name;
            condominium.Address = request.Address;
            condominium.Units = request.Units;
            condominium.Floors = request.Floors;
            condominium.HasElevator = request.HasElevator;
            condominium.HasSwimmingPool = request.HasSwimmingPool;
            condominium.HasPartyRoom = request.HasPartyRoom;
            condominium.HasPlayground = request.HasPlayground;
            condominium.HasFitnessRoom = request.HasFitnessRoom;
            condominium.CondominiumValue = request.CondominiumValue;
            condominium.UpdatedAt = DateTime.UtcNow;

            _context.Condominiums.Update(condominium);
            await _context.SaveChangesAsync();

            return new Response<Condominium?>(condominium);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível atualizar o condomínio");
        }
    }

    /// <summary>
    /// Realiza a exclusão lógica de um condomínio, marcando-o como inativo.
    /// </summary>
    /// <remarks>
    /// Este método altera o status de um condomínio para inativo no banco de dados.    
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do condomínio a ser excluído.</param>
    /// <returns>Retorna a resposta com o condomínio atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Condominium?>> DeleteAsync(DeleteCondominiumRequest request)
    {
        try
        {
            var condominium = await _context
                .Condominiums
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            if (condominium is null)
                return new Response<Condominium?>(null, 404, "Condomínio não encontrado");

            condominium.IsActive = false;
            condominium.UpdatedAt = DateTime.UtcNow;

            _context.Condominiums.Update(condominium);
            await _context.SaveChangesAsync();

            return new Response<Condominium?>(condominium);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível deletar o condomínio");
        }
    }

    /// <summary>
    /// Obtém um condomínio específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método busca um condomínio no banco de dados com base no ID fornecido.    
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do condomínio desejado.</param>
    /// <returns>Retorna o objeto do condomínio ou um erro caso não seja encontrado.</returns>
    public async Task<Response<Condominium?>> GetByIdAsync(GetCondominiumByIdRequest request)
    {
        try
        {
            var condominium = await _context
                .Condominiums
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive);

            return condominium is null
                ? new Response<Condominium?>(null, 404, "Condomínio não encontrado")
                : new Response<Condominium?>(condominium);
        }
        catch
        {
            return new Response<Condominium?>(null, 500, "Não foi possível buscar o condomínio");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todos os condomínios ativos, com opção de filtro por busca.
    /// </summary>
    /// <remarks>
    /// <para> Este método busca todos os condomínios ativos no banco de dados e aplica
    /// filtros de busca, se fornecidos.  </para>
    /// <para> A resposta é paginada com base nos parâmetros
    /// de paginação fornecidos na requisição e retorna uma lista de condomínios.</para>
    /// <para>Caso o filtro não seja fornecido, retorna todos os condomínios ativos.</para>    
    /// </remarks>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com os condomínios ativos com base no filtro ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Condominium>?>> GetAllAsync(GetAllCondominiumsRequest request)
    {
        try
        {
            var query = _context
                .Condominiums
                .AsNoTracking()
                .Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(request.FilterBy))
                query = query.FilterByProperty(request.SearchTerm, request.FilterBy);

            var count = await query.CountAsync();

            var condominiums = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<List<Condominium>?>(condominiums, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Condominium>?>(null, 500, "Não foi possível buscar os condomínios");
        }
    }
}
