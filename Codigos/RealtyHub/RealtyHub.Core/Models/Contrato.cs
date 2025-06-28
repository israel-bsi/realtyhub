using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa um contrato no sistema.
/// </summary>
/// <remarks>
/// Essa classe herda de <c><see cref="Entidade"/></c>,
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class Contrato : Entidade
{
    /// <summary>
    /// Obtém ou define o ID do contrato.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do contrato.</value>
    public long Id { get; set; }

    /// <summary>
    /// Obtém ou define o ID do vendedor associado ao contrato.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do vendedor.</value>
    public long VendedorId { get; set; }

    /// <summary>
    /// Obtém ou define o ID do comprador associado ao contrato.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do comprador.</value>
    public long CompradorId { get; set; }

    /// <summary>
    /// Obtém ou define o ID da oferta associada ao contrato.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da oferta.</value>
    public long PropostaId { get; set; }

    /// <summary>
    /// Obtém ou define a data de emissão do contrato.
    /// </summary>
    /// <value>Uma data representando quando o contrato foi emitido.</value>
    [Required(ErrorMessage = "A data de emissão é obrigatoria")]
    public DateTime? DataEmissao { get; set; }

    /// <summary>
    /// Obtém ou define a data de vigência do contrato.
    /// </summary>
    /// <value>Uma data representando quando o contrato entra em vigor.</value>
    [Required(ErrorMessage = "A data de vigência é obrigatoria")]
    public DateTime? DataVigencia { get; set; }

    /// <summary>
    /// Obtém ou define a data de término do contrato.
    /// </summary>
    /// <value>Uma data representando quando o contrato termina.</value>
    [Required(ErrorMessage = "A data de término é obrigatoria")]
    public DateTime? DataTermino { get; set; }

    /// <summary>
    /// Obtém ou define a data de assinatura do contrato.
    /// </summary>
    /// <value>Uma data representando quando o contrato foi assinado.</value>
    public DateTime? DataAssinatura { get; set; }

    /// <summary>
    /// Obtém ou define o ID do arquivo associado ao contrato.
    /// </summary>
    /// <value>Uma string representando o ID do arquivo.</value>
    public string ArquivoId { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o contrato está ativo.
    /// </summary>
    /// <value><c>true</c> se o contrato estiver ativo; caso contrário, <c>false</c>.</value>
    public bool Ativo { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado ao contrato.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a oferta associada ao contrato.
    /// </summary>
    /// <value>Um objeto <see cref="Proposta"/> representando a oferta. Pode ser nulo.</value>
    public Proposta? Proposta { get; set; } = new();

    /// <summary>
    /// Obtém ou define o cliente vendedor associado ao contrato.
    /// </summary>
    /// <value>Um objeto <see cref="Customer"/> representando o vendedor. Pode ser nulo.</value>
    public Cliente? Vendedor { get; set; }

    /// <summary>
    /// Obtém ou define o cliente comprador associado ao contrato.
    /// </summary>
    /// <value>Um objeto <see cref="Customer"/> representando o comprador. Pode ser nulo.</value>
    public Cliente? Comprador { get; set; }

    /// <summary>
    /// Obtém o caminho completo para o arquivo PDF do contrato.
    /// </summary>
    /// <value>Uma string representando a URL completa onde o PDF do contrato está disponível.</value>
    public string CaminhoArquivo =>
        $"{Configuration.BackendUrl}/contracts/{ArquivoId}.pdf";
}