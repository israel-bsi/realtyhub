using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

public class ContractTemplatesHandler(AppDbContext context) : IContractTemplateHandler
{
    public async Task<Response<List<ContractTemplate>?>> GetAllAsync()
    {
        try
        {
            var contracts = await context
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