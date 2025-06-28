using System.Globalization;
using RealtyHub.Core.Extensions;
using RealtyHub.Core.Models;

namespace RealtyHub.ApiService.Services;

/// <summary>
/// Classe responsável por mapear as variáveis de contrato para substituição em modelos.
/// </summary>
/// <remarks>
/// <para>A classe <c><see cref="VariablesContractMappings"/></c> 
/// gera um dicionário de campos que serão utilizados para substituir 
/// as variáveis em um modelo de contrato.</para>
/// <para>Ela concatena campos padrão, de pessoa jurídica e pessoa física, 
/// além dos dados dos pagamentos.</para>
/// </remarks>
public class VariablesContractMappings
{
    /// <summary>
    /// Retorna um dicionário com todas as variáveis mapeadas para o contrato.
    /// </summary>
    /// <remarks>
    /// Este método concatena o dicionário de campos padrão, os campos de pessoa jurídica, os campos de pessoa física 
    /// e os textos referentes aos pagamentos, retornando um dicionário que mapeia cada variável ao seu valor correspondente.
    /// </remarks>
    /// <returns>
    /// Um objeto <c><see cref="Dictionary{TKey, TValue}"/></c> contendo as chaves e os respectivos valores para substituição.
    /// </returns>
    public Dictionary<string, string> GetFields()
    {
        var paymentsFields = GetPaymentsText(_contrato.Proposta!.Pagamentos);
        return _defaultFields
            .Concat(_fieldsPj)
            .Concat(_fieldsPf)
            .Concat(paymentsFields)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Dicionário contendo os campos padrão mapeados para o contrato.
    /// </summary>
    private readonly Dictionary<string, string> _defaultFields;

    /// <summary>
    /// Dicionário contendo os campos de pessoa jurídica mapeados para o contrato.
    /// </summary>
    private readonly Dictionary<string, string> _fieldsPj;

    /// <summary>
    /// Dicionário contendo os campos de pessoa física mapeados para o contrato.
    /// </summary>
    private readonly Dictionary<string, string> _fieldsPf;

    /// <summary>
    /// Objeto do tipo <c><see cref="Contrato"/></c> que contém os dados do contrato.
    /// </summary>
    private readonly Contrato _contrato;

    /// <summary>
    /// Inicializa uma nova instância de <c><see cref="VariablesContractMappings"/></c> com o contrato especificado.
    /// </summary>
    /// <remarks>
    /// O construtor recebe um objeto do tipo <c><see cref="Contrato"/></c> e inicializa os dicionários de campos padrão,
    /// de pessoa jurídica e de pessoa física, mapeando os dados pertinentes do contrato.
    /// </remarks>
    /// <param name="contrato">Um objeto do tipo <c><see cref="Contrato"/></c> contendo os dados do contrato.</param>
    public VariablesContractMappings(Contrato contrato)
    {
        _contrato = contrato;
        _defaultFields = new Dictionary<string, string>
        {
            { "enderecoCompleto_cliente_proprietario",
                $"Rua: {contrato.Vendedor!.Endereco.Logradouro}, " +
                (string.IsNullOrEmpty(contrato.Vendedor.Endereco.Complemento) ? ""
                    : $"Complemento: {contrato.Vendedor.Endereco.Complemento}, ") +
                $"Número: {contrato.Vendedor.Endereco.Numero}, " +
                $"Bairro: {contrato.Vendedor.Endereco.Bairro}, " +
                $"Cidade: {contrato.Vendedor.Endereco.Cidade}, " +
                $"Estado: {contrato.Vendedor.Endereco.Estado}, " +
                $"País: {contrato.Vendedor.Endereco.Pais}, " +
                $"CEP: {contrato.Vendedor.Endereco.Cep} "
            },
            { "enderecoCompleto_cliente_comprador",
                $"Rua: {contrato.Comprador!.Endereco.Logradouro}, " +
                (string.IsNullOrEmpty(contrato.Comprador.Endereco.Complemento) ? ""
                    : $"Complemento: {contrato.Comprador.Endereco.Complemento}, ") +
                $"Número: {contrato.Comprador.Endereco.Numero}, " +
                $"Bairro: {contrato.Comprador.Endereco.Bairro}, " +
                $"Cidade: {contrato.Comprador.Endereco.Cidade}, " +
                $"Estado: {contrato.Comprador.Endereco.Estado}, " +
                $"País: {contrato.Comprador.Endereco.Pais}, " +
                $"CEP: {contrato.Comprador.Endereco.Cep} "
            },
            { "tipo_imovel", contrato.Proposta!.Imovel!.TipoImovel.GetDisplayName() },
            { "enderecoCompleto_imovel",
                $"Rua: {contrato.Proposta.Imovel!.Endereco.Logradouro}, " +
                (string.IsNullOrEmpty(contrato.Proposta.Imovel!.Endereco.Complemento) ? ""
                    : $"Complemento: {contrato.Proposta.Imovel!.Endereco.Complemento}, ") +
                $"Número: {contrato.Proposta.Imovel!.Endereco.Numero}, " +
                $"Bairro: {contrato.Proposta.Imovel!.Endereco.Bairro}, " +
                $"Cidade: {contrato.Proposta.Imovel!.Endereco.Cidade}, " +
                $"Estado: {contrato.Proposta.Imovel!.Endereco.Estado}, " +
                $"País: {contrato.Proposta.Imovel!.Endereco.Pais}, " +
                $"CEP: {contrato.Proposta.Imovel!.Endereco.Cep} "
            },
            { "area_imovel", contrato.Proposta!.Imovel!.Area.ToString(CultureInfo.CurrentCulture) },
            { "numeroMatriculaCartorio_imovel", contrato.Proposta!.Imovel!.NumeroRegistro },
            { "cartorioRegistro_imovel", contrato.Proposta!.Imovel!.RegistroCartorio },
            { "valorOferecido_proposta", contrato.Proposta!.Valor.ToString(CultureInfo.CurrentCulture) },
            { "cidade_imovel", contrato.Proposta!.Imovel!.Endereco.Cidade },
            { "estado_imovel", contrato.Proposta!.Imovel!.Endereco.Estado },
            { "data_assinatura_contrato", contrato.DataAssinatura?.ToString("dd/MM/yyyy")! },
        };
        _fieldsPj = new Dictionary<string, string>
        {
            { "razaoSocial_cliente_proprietario", contrato.Vendedor!.NomeFantasia },
            { "cnpj_cliente_proprietario", contrato.Vendedor.NumeroDocumento },
            { "razaoSocial_cliente_comprador", contrato.Comprador!.NomeFantasia },
            { "cnpj_cliente_comprador", contrato.Comprador!.NumeroDocumento }
        };
        _fieldsPf = new Dictionary<string, string>
        {
            { "nome_cliente_proprietario", contrato.Vendedor.Nome },
            { "nome_cliente_comprador", contrato.Comprador.Nome },
            { "nacionalidade_cliente_proprietario", contrato.Vendedor.Nacionalidade },
            { "nacionalidade_cliente_comprador", contrato.Comprador.Nacionalidade },
            { "estadoCivil_cliente_proprietario", contrato.Vendedor.TipoStatusCivil.GetDisplayName() },
            { "estadoCivil_cliente_comprador", contrato.Comprador.TipoStatusCivil.GetDisplayName() },
            { "profissao_cliente_proprietario", contrato.Vendedor.Ocupacao },
            { "profissao_cliente_comprador", contrato.Comprador.Ocupacao },
            { "cpf_cliente_proprietario", contrato.Vendedor.NumeroDocumento },
            { "cpf_cliente_comprador", contrato.Comprador.NumeroDocumento },
            { "rg_cliente_proprietario", contrato.Vendedor.Rg },
            { "rg_cliente_comprador", contrato.Comprador.Rg },
            { "orgaoExpeditorRg_cliente_proprietario", contrato.Vendedor.AutoridadeEmissora },
            { "orgaoExpeditorRg_cliente_comprador", contrato.Comprador.AutoridadeEmissora },
            { "dataemissaorg_cliente_proprietario", contrato.Vendedor.DataEmissaoRg?.ToString("dd/MM/yyyy")! },
            { "dataemissaorg_cliente_comprador", contrato.Comprador.DataEmissaoRg?.ToString("dd/MM/yyyy")! }
        };
    }

    /// <summary>
    /// Gera uma representação textual dos pagamentos do contrato.
    /// </summary>
    /// <remarks>
    /// Este método percorre a lista de pagamentos do contrato, criando um mapeamento de forma 
    /// que cada pagamento seja representado por uma chave no formato "formaPagamentoX", onde X é um índice.
    /// Caso haja menos de 5 pagamentos, as chaves restantes são preenchidas com uma string vazia.
    /// </remarks>
    /// <param name="payments">A lista de pagamentos do contrato.</param>
    /// <returns>
    /// Um objeto <c><see cref="Dictionary{TKey, TValue}"/></c> representando os pagamentos mapeados para substituição.
    /// </returns>
    private Dictionary<string, string> GetPaymentsText(List<Pagamento> payments)
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