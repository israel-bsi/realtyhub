using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EViewingStatus
{
    [Display(Name = "Agendado")]
    Scheduled = 1,
    [Display(Name = "Finalizado")]
    Done = 2,
    [Display(Name = "Cancelado")]
    Canceled = 3
}