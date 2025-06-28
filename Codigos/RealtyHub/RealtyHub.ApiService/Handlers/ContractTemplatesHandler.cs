using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas aos modelos de contrato.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IContractTemplateHandler"/> e fornece
/// métodos para buscar modelos de contrato no banco de dados.
/// /// </remarks>
public class ContractTemplatesHandler : IContractTemplateHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com modelos de contrato.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nos modelos de contratos.
    /// </remarks>    
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ContractTemplatesHandler"/>.
    /// </summary>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nos modelos de contrato.
    /// </remarks>
    /// <param name="context">Contexto do banco de dados para interação com os modelos de contrato.</param>
    public ContractTemplatesHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtém todos os modelos de contrato disponíveis no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método busca todos os modelos de contrato e retorna uma lista com os resultados.
    /// </remarks>
    /// <returns>Retorna uma lista de modelos de contrato ou um erro em caso de falha.</returns>
    public async Task<Response<List<ModeloContrato>?>> GetAllAsync()
    {
        try
        {
            var contracts = await _context
                .ContractTemplates
                .AsNoTracking()
                .ToListAsync();

            return new Response<List<ModeloContrato>?>(contracts);
        }
        catch
        {
            return new Response<List<ModeloContrato>?>(null, 500, "Não foi possível buscar os modelos de contrato");
        }
    }
}
