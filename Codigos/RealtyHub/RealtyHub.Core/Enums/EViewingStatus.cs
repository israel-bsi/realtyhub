using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

public enum EViewingStatus
{
    [Display(Name = "Agendada")]
    Scheduled = 1,
    [Display(Name = "Finalizada")]
    Done = 2,
    [Display(Name = "Cancelada")]
    Canceled = 3
}