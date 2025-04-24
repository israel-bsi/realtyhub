using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ContractTemplatesHandler : IContractTemplateHandler
{
    private readonly AppDbContext _context;

    public ContractTemplatesHandler(AppDbContext context)
    {
        _context = context;
    }

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