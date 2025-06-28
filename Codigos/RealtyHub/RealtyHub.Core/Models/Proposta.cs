using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma proposta de compra de um imóvel no sistema.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Entidade"/></c>, 
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class Proposta : Entidade
{
    /// <summary>
    /// Obtém ou define o ID da proposta.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da proposta.</value>
    public long Id { get; set; }

    /// <summary>
    /// Obtém ou define o valor da proposta.
    /// </summary>
    /// <value>Um valor decimal representando o montante da proposta.</value>
    [Required(ErrorMessage = "O valor da proposta é obrigatório")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Obtém ou define o ID do imóvel associado à proposta.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do imóvel.</value>
    [Required(ErrorMessage = "O imóvel é obrigatório")]
    public long ImovelId { get; set; }

    /// <summary>
    /// Obtém ou define o ID do cliente que está fazendo a proposta.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do cliente.</value>
    [Required(ErrorMessage = "O cliente é obrigatório")]
    public long CompradorId { get; set; }

    /// <summary>
    /// Obtém ou define a lista de pagamentos relacionados à proposta.
    /// </summary>
    /// <value>Uma lista de objetos <see cref="Pagamento"/> representando os pagamentos.</value>
    public List<Pagamento> Pagamentos { get; set; } = [];

    /// <summary>
    /// Obtém ou define a data de submissão da proposta.
    /// </summary>
    /// <value>Uma data representando quando a proposta foi submetida.</value>
    public DateTime? DataDeEnvio { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Obtém ou define o status da proposta.
    /// </summary>
    /// <value>Um valor do enum <see cref="EStatusProposta"/> representando o status atual da proposta.</value>
    public EStatusProposta StatusProposta { get; set; } = EStatusProposta.EmAnalise;

    /// <summary>
    /// Obtém ou define o ID do usuário associado à proposta.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UsuarioId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define o cliente associado à proposta.
    /// </summary>
    /// <value>Um objeto <see cref="Cliente"/> representando o cliente. Pode ser nulo.</value>
    public Cliente? Comprador { get; set; }

    /// <summary>
    /// Obtém ou define o imóvel associado à proposta.
    /// </summary>
    /// <value>Um objeto <see cref="Imovel"/> representando o imóvel. Pode ser nulo.</value>
    public Imovel? Imovel { get; set; }

    /// <summary>
    /// Obtém ou define o contrato associado à proposta.
    /// </summary>
    /// <value>Um objeto <see cref="Contrato"/> representando o contrato. Pode ser nulo.</value>
    public Contrato? Contrato { get; set; }
}