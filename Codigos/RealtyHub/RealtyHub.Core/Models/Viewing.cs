using RealtyHub.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Models;

public class Viewing : Entity
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Data da visita é um campo obrigatório")]
    [DataType(DataType.DateTime)]
    public DateTime? ViewingDate { get; set; } = DateTime.Now;
    [Required(ErrorMessage = "Status da visita é um campo obrigatório")]
    public EViewingStatus ViewingStatus { get; set; } = EViewingStatus.Scheduled;
    [Required(ErrorMessage = "Cliente é um campo obrigatório")]
    public long BuyerId { get; set; }
    [Required(ErrorMessage = "Propriedade é um campo obrigatório")]
    public long PropertyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Property? Property { get; set; }
    public Customer? Buyer { get; set; }
}