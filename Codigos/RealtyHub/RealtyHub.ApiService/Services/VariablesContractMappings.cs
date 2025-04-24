using System.Globalization;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services;

public class VariablesContractMappings
{
    public Dictionary<string, string> GetFields()
    {
        var paymentsFields = GetPaymentsText(_contract.Offer!.Payments);
        return _defaultFields
            .Concat(_fieldsPj)
            .Concat(_fieldsPf)
            .Concat(paymentsFields)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private readonly Dictionary<string, string> _defaultFields;

    private readonly Dictionary<string, string> _fieldsPj;

    private readonly Dictionary<string, string> _fieldsPf;

    private readonly Contract _contract;

    public VariablesContractMappings(Contract contract)
    {
        _contract = contract;
        _defaultFields = new Dictionary<string, string>
        {
            { "enderecoCompleto_cliente_proprietario",
                $"Rua: {contract.Seller!.Address.Street}, " +
                (string.IsNullOrEmpty(contract.Seller.Address.Complement) ? ""
                    : $"Complemento: {contract.Seller.Address.Complement}, ") +
                $"Número: {contract.Seller.Address.Number}, " +
                $"Bairro: {contract.Seller.Address.Neighborhood}, " +
                $"Cidade: {contract.Seller.Address.City}, " +
                $"Estado: {contract.Seller.Address.State}, " +
                $"País: {contract.Seller.Address.Country}, " +
                $"CEP: {contract.Seller.Address.ZipCode} "
            },
            { "enderecoCompleto_cliente_comprador",
                $"Rua: {contract.Buyer!.Address.Street}, " +
                (string.IsNullOrEmpty(contract.Buyer.Address.Complement) ? ""
                    : $"Complemento: {contract.Buyer.Address.Complement}, ") +
                $"Número: {contract.Buyer.Address.Number}, " +
                $"Bairro: {contract.Buyer.Address.Neighborhood}, " +
                $"Cidade: {contract.Buyer.Address.City}, " +
                $"Estado: {contract.Buyer.Address.State}, " +
                $"País: {contract.Buyer.Address.Country}, " +
                $"CEP: {contract.Buyer.Address.ZipCode} "
            },
            { "tipo_imovel", contract.Offer!.Property!.PropertyType.GetDisplayName()},
            { "enderecoCompleto_imovel",
                $"Rua: {contract.Offer.Property!.Address.Street}, " +
                (string.IsNullOrEmpty(contract.Offer.Property!.Address.Complement) ? ""
                    : $"Complemento: {contract.Offer.Property!.Address.Complement}, ") +
                $"Número: {contract.Offer.Property!.Address.Number}, " +
                $"Bairro: {contract.Offer.Property!.Address.Neighborhood}, " +
                $"Cidade: {contract.Offer.Property!.Address.City}, " +
                $"Estado: {contract.Offer.Property!.Address.State}, " +
                $"País: {contract.Offer.Property!.Address.Country}, " +
                $"CEP: {contract.Offer.Property!.Address.ZipCode} "
            },
            { "area_imovel", contract.Offer!.Property!.Area.ToString(CultureInfo.CurrentCulture) },
            { "numeroMatriculaCartorio_imovel", contract.Offer!.Property!.RegistryNumber },
            { "cartorioRegistro_imovel", contract.Offer!.Property!.RegistryRecord },
            { "valorOferecido_proposta", contract.Offer!.Amount.ToString(CultureInfo.CurrentCulture) },
            { "cidade_imovel", contract.Offer!.Property!.Address.City },
            { "estado_imovel", contract.Offer!.Property!.Address.State },
            { "data_assinatura_contrato", contract.SignatureDate?.ToString("dd/MM/yyyy")! },
        };
        _fieldsPj = new Dictionary<string, string>
        {
            { "razaoSocial_cliente_proprietario", contract.Seller!.BusinessName },
            { "cnpj_cliente_proprietario", contract.Seller.DocumentNumber },
            { "razaoSocial_cliente_comprador", contract.Buyer!.BusinessName },
            { "cnpj_cliente_comprador", contract.Buyer!.DocumentNumber }
        };
        _fieldsPf = new Dictionary<string, string>
        {
            { "nome_cliente_proprietario", contract.Seller.Name },
            { "nome_cliente_comprador", contract.Buyer.Name },
            { "nacionalidade_cliente_proprietario", contract.Seller.Nationality },
            { "nacionalidade_cliente_comprador", contract.Buyer.Nationality },
            { "estadoCivil_cliente_proprietario", contract.Seller.MaritalStatus.GetDisplayName() },
            { "estadoCivil_cliente_comprador", contract.Buyer.MaritalStatus.GetDisplayName() },
            { "profissao_cliente_proprietario", contract.Seller.Occupation },
            { "profissao_cliente_comprador", contract.Buyer.Occupation },
            { "cpf_cliente_proprietario", contract.Seller.DocumentNumber },
            { "cpf_cliente_comprador", contract.Buyer.DocumentNumber },
            { "rg_cliente_proprietario", contract.Seller.Rg },
            { "rg_cliente_comprador", contract.Buyer.Rg },
            { "orgaoExpeditorRg_cliente_proprietario", contract.Seller.IssuingAuthority },
            { "orgaoExpeditorRg_cliente_comprador", contract.Seller.IssuingAuthority },
            { "dataemissaorg_cliente_proprietario", contract.Seller.RgIssueDate?.ToString("dd/MM/yyyy")! },
            { "dataemissaorg_cliente_comprador", contract.Seller.RgIssueDate?.ToString("dd/MM/yyyy")! }
        };
    }

    private Dictionary<string, string> GetPaymentsText(List<Payment> payments)
    {
        var dictPayments = new Dictionary<string, string>();
        var index = 1;
        foreach (var payment in payments)
        {
            dictPayments.Add($"formaPagamento{index}", payment.ToString());
            index++;
        }

        for (var i = index; i <= 5; i++)
        {
            dictPayments.Add($"formaPagamento{i}", "");
        }
        return dictPayments;
    }
}