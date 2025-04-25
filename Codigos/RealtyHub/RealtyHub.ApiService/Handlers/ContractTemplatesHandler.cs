using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas aos modelos de contrato.
/// </summary>
public class ContractTemplatesHandler : IContractTemplateHandler
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ContractTemplatesHandler"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados para interação com os modelos de contrato.</param>
    public ContractTemplatesHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém todos os modelos de contrato disponíveis no banco de dados.
    /// </summary>
    /// <returns>Retorna uma lista de modelos de contrato ou um erro em caso de falha.</returns>
    public async Task<Response<List<ContractTemplate>?>> GetAllAsync()
    {
        try
        {
            var contracts = await _context
                .ContractTemplates
                .AsNoTracking()
                .ToListAsync();

            return new Response<List<ContractTemplate>?>(contracts);
        }
        catch
        {
            return new Response<List<ContractTemplate>?>(null, 500, "Não foi possível buscar os modelos de contrato");
        }
    }
}
