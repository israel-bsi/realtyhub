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
    public long CustomerId { get; set; }
    [Required(ErrorMessage = "Propriedade é um campo obrigatório")]
    public long PropertyId { get; set; }
    public Property Property { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}