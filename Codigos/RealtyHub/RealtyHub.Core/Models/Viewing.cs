using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

/// <summary>
/// Representa uma visita a um imóvel no sistema.
/// </summary>
/// <remarks>
/// Esta classe herda de <c><see cref="Entity"/></c>, 
/// que contém propriedades comuns a todas as entidades do sistema.
/// </remarks>
public class Viewing : Entity
{
    /// <summary>
    /// Obtém ou define o ID da visita.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da visita.</value>
    public long Id { get; set; }

    /// <summary>
    /// Obtém ou define a data e hora da visita.
    /// </summary>
    /// <value>Uma data/hora representando quando a visita ocorrerá. O valor padrão é a data e hora atual.</value>
    [Required(ErrorMessage = "Data da visita é um campo obrigatório")]
    [DataType(DataType.DateTime)]
    public DateTime? ViewingDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Obtém ou define o status da visita.
    /// </summary>
    /// <value>Um valor do enum <see cref="EViewingStatus"/> representando o status da visita. O padrão é <see cref="EViewingStatus.Scheduled"/>.</value>
    [Required(ErrorMessage = "Status da visita é um campo obrigatório")]
    public EViewingStatus ViewingStatus { get; set; } = EViewingStatus.Scheduled;

    /// <summary>
    /// Obtém ou define o ID do comprador associado à visita.
    /// </summary>
    /// <value>Um valor inteiro representando o ID do comprador.</value>
    [Required(ErrorMessage = "Cliente é um campo obrigatório")]
    public long BuyerId { get; set; }

    /// <summary>
    /// Obtém ou define o ID da imóvel que será visitada.
    /// </summary>
    /// <value>Um valor inteiro representando o ID da imóvel.</value>
    [Required(ErrorMessage = "Imóvel é um campo obrigatório")]
    public long PropertyId { get; set; }

    /// <summary>
    /// Obtém ou define o ID do usuário associado a esta visita.
    /// </summary>
    /// <value>Uma string representando o ID do usuário.</value>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a imóvel que será visitada.
    /// </summary>
    /// <value>Um objeto <see cref="Property"/> representando a imóvel. Pode ser nulo.</value>
    public Property? Property { get; set; }

    /// <summary>
    /// Obtém ou define o cliente comprador associado à visita.
    /// </summary>
    /// <value>Um objeto <see cref="Customer"/> representando o comprador. Pode ser nulo.</value>
    public Customer? Buyer { get; set; }
}