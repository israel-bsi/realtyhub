using Microsoft.EntityFrameworkCore;
using RealtyHub.ApiService.Data;
using RealtyHub.ApiService.Services;
using RealtyHub.Core.Enums;
using RealtyHub.Core.Handlers;
using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Contracts;
using RealtyHub.Core.Responses;

namespace RealtyHub.ApiService.Handlers;

/// <summary>
/// Responsável pelas operações relacionadas a contratos.
/// </summary>
/// <remarks>
/// Esta classe implementa a interface <see cref="IContractHandler"/> e fornece
/// métodos para criar, atualizar, deletar e buscar contratos no banco de dados.
/// </remarks>
public class ContractHandler : IContractHandler
{
    /// <summary>
    /// Contexto do banco de dados para interação com contratos.
    /// </summary>
    /// <remarks>
    /// Este campo é utilizado para realizar operações CRUD nos contratos.
    /// </remarks>
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ContractHandler"/>.
    /// </summary>
    /// <param name="context">Contexto do banco de dados para interação com contratos.</param>
    /// <remarks>
    /// Este construtor é utilizado para injetar o contexto do banco de dados
    /// necessário para realizar operações CRUD nos contratos.
    /// </remarks>
    public ContractHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo contrato no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método cria um novo contrato baseado em uma proposta aceita, 
    /// associando comprador, vendedor e dados adicionais.
    /// </remarks>
    /// <param name="request">Objeto contendo as informações para criação do contrato.</param>
    /// <returns>Retorna uma resposta com o contrato criado e o código de status.</returns>
    public async Task<Response<Contract?>> CreateAsync(Contract request)
    {
        try
        {
            var offer = await _context
                .Offers
                .Include(o => o.Buyer)
                .Include(o => o.Property)
                .ThenInclude(o => o!.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

            if (offer is null)
                return new Response<Contract?>(null, 404, "Proposta não encontrada");

            if (offer.OfferStatus != EOfferStatus.Accepted)
                return new Response<Contract?>(null, 400, "A proposta precisa estar aceita para criar um contrato");

            _context.Attach(offer);

            var contract = new Contract
            {
                IssueDate = request.IssueDate,
                SignatureDate = request.SignatureDate,
                EffectiveDate = request.EffectiveDate,
                TermEndDate = request.TermEndDate,
                FileId = Guid.NewGuid().ToString(),
                Buyer = offer.Buyer,
                BuyerId = offer.Buyer!.Id,
                SellerId = offer.Property!.SellerId,
                Seller = offer.Property.Seller,
                Offer = offer,
                OfferId = offer.Id,
                IsActive = true,
                UserId = request.UserId
            };

            await CreateOrUpdateContract(contract, false);

            await _context.Contracts.AddAsync(contract);
            await _context.SaveChangesAsync();

            return new Response<Contract?>(contract, 201, "Contrato criado com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível criar o contrato");
        }
    }

    /// <summary>
    /// Atualiza um contrato existente no banco de dados.
    /// </summary>
    /// <remarks>
    /// Este método busca o contrato e ajusta as informações conforme a requisição fornecida.
    /// </remarks>
    /// <param name="request">Objeto contendo as novas informações do contrato.</param>
    /// <returns>Retorna a resposta com o contrato atualizado ou um erro se não for encontrado.</returns>
    public async Task<Response<Contract?>> UpdateAsync(Contract request)
    {
        try
        {
            var contract = await _context
                .Contracts
                .Include(o => o.Seller)
                .Include(o => o.Buyer)
                .Include(o => o.Offer)
                .ThenInclude(o => o!.Property)
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            var offer = await _context
                .Offers
                .Include(offer => offer.Property)
                .ThenInclude(property => property!.Seller)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == request.OfferId);

            if (offer is null)
                return new Response<Contract?>(null, 404, "Proposta não encontrada");

            contract.IssueDate = request.IssueDate;
            contract.SignatureDate = request.SignatureDate;
            contract.EffectiveDate = request.EffectiveDate;
            contract.TermEndDate = request.TermEndDate;
            contract.Buyer = request.Buyer;
            contract.BuyerId = request.BuyerId;
            contract.SellerId = offer.Property!.SellerId;
            contract.Seller = offer.Property.Seller;
            contract.Offer = offer;
            contract.OfferId = request.OfferId;
            contract.FileId = request.FileId;
            contract.IsActive = request.IsActive;
            contract.UpdatedAt = DateTime.UtcNow;

            await CreateOrUpdateContract(contract, true);

            await _context.SaveChangesAsync();

            return new Response<Contract?>(contract, 200, "Contrato atualizado com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível atualizar o contrato");
        }
    }

    /// <summary>
    /// Realiza a exclusão lógica de um contrato, marcando-o como inativo.
    /// </summary>
    /// <remarks>
    /// Não remove o registro do banco, apenas altera seu status de ativo para inativo.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do contrato a ser excluído.</param>
    /// <returns>Retorna a resposta com o status da exclusão ou um erro se não for encontrado.</returns>
    public async Task<Response<Contract?>> DeleteAsync(DeleteContractRequest request)
    {
        try
        {
            var contract = await _context
                .Contracts
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            contract.IsActive = false;
            contract.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new Response<Contract?>(null, 204, "Contrato excluído com sucesso");
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível excluir o contrato");
        }
    }

    /// <summary>
    /// Obtém um contrato específico pelo ID.
    /// </summary>
    /// <remarks>
    /// Este método localiza o contrato correspondente ao ID fornecido 
    /// e retorna o objeto completo, se encontrado.
    /// </remarks>
    /// <param name="request">Requisição que contém o ID do contrato desejado.</param>
    /// <returns>Retorna o objeto do contrato ou um erro caso não seja encontrado.</returns>
    public async Task<Response<Contract?>> GetByIdAsync(GetContractByIdRequest request)
    {
        try
        {
            var contract = await _context
                .Contracts
                .AsNoTracking()
                .Include(c => c.Offer)
                .ThenInclude(o => o!.Property)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
                .FirstOrDefaultAsync(c => c.Id == request.Id
                                          && c.UserId == request.UserId
                                          && c.IsActive);

            if (contract is null)
                return new Response<Contract?>(null, 404, "Contrato não encontrado");

            contract.EffectiveDate = contract.EffectiveDate?.ToLocalTime();
            contract.IssueDate = contract.IssueDate?.ToLocalTime();
            contract.SignatureDate = contract.SignatureDate?.ToLocalTime();
            contract.TermEndDate = contract.TermEndDate?.ToLocalTime();

            return new Response<Contract?>(contract);
        }
        catch
        {
            return new Response<Contract?>(null, 500, "Não foi possível encontrar o contrato");
        }
    }

    /// <summary>
    /// Retorna uma lista paginada de todos os contratos ativos, com opção de filtro por data.
    /// </summary>
    /// <remarks>
    /// <para>Este método aplica paginação, filtra por data de emissão e
    /// retorna apenas contratos marcados como ativos. </para>
    /// <para> Caso o filtro não seja fornecido, retorna todos os contratos ativos.</para>
    /// </remarks>
    /// <param name="request">Requisição que contém parâmetros de paginação e filtro.</param>
    /// <returns>Retorna uma resposta paginada com os contratos ativos ou um erro em caso de falha.</returns>
    public async Task<PagedResponse<List<Contract>?>> GetAllAsync(GetAllContractsRequest request)
    {
        try
        {
            var query = _context
                .Contracts
                .AsNoTracking()
                .Include(c => c.Offer)
                .ThenInclude(o => o!.Property)
                .Include(c => c.Buyer)
                .Include(c => c.Seller)
                .Where(c => c.UserId == request.UserId && c.IsActive);

            if (request.StartDate is not null && request.EndDate is not null)
            {
                var startDate = DateTime.Parse(request.StartDate).ToUniversalTime();
                var endDate = DateTime.Parse(request.EndDate).ToUniversalTime();

                query = query.Where(v => v.IssueDate >= startDate
                                         && v.IssueDate <= endDate);
            }

            var contracts = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                contract.EffectiveDate = contract.EffectiveDate?.ToLocalTime();
                contract.IssueDate = contract.IssueDate?.ToLocalTime();
                contract.SignatureDate = contract.SignatureDate?.ToLocalTime();
                contract.TermEndDate = contract.TermEndDate?.ToLocalTime();
            }

            var count = await query.CountAsync();

            return new PagedResponse<List<Contract>?>(contracts, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Contract>?>(null, 500, "Não foi possível retornar os contratos");
        }
    }

    /// <summary>
    /// Gera ou atualiza o arquivo de contrato com base nas informações fornecidas e no modelo apropriado.
    /// </summary>
    /// <remarks>
    /// Este método carrega o modelo correto, atualiza o documento de contrato e salva em disco.
    /// </remarks>
    /// <param name="contract">Objeto do contrato a ser gerado ou atualizado.</param>
    /// <param name="update">Indica se o contrato está sendo atualizado.</param>
    private async Task CreateOrUpdateContract(Contract contract, bool update)
    {
        var contractModelType = contract.Buyer?.PersonType switch
        {
            EPersonType.Business when contract.Seller?.PersonType == EPersonType.Business
                => EContractModelType.PJPJ,
            EPersonType.Individual when contract.Seller?.PersonType == EPersonType.Individual
                => EContractModelType.PFPF,
            EPersonType.Business when contract.Seller?.PersonType == EPersonType.Individual
                => EContractModelType.PFPJ,
            _ => EContractModelType.PJPF
        };

        var contractModel = await _context
            .ContractTemplates
            .FirstOrDefaultAsync(cm => cm.Type == contractModelType);

        var docxContractGenerator = new ContractGenerator();
        var pathContactFile = Path.Combine(Configuration.ContractsPath, $"{contract.Id}.docx");
        if (update)
            File.Delete(pathContactFile);
        docxContractGenerator.GenerateContract(contract, contractModel);
    }
}