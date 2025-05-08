using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

/// <summary>
/// Enumerador para os status de visita.
/// </summary>
/// <remarks>
/// <para>O enum representa os diferentes status que uma visita pode assumir na aplicação.</para>
/// <para>Cada valor do enum é associado a um número inteiro e possui um atributo 
/// Display que fornece uma descrição legível para o status.</para>
/// </remarks>
public enum EViewingStatus
{
    /// <summary>
    /// Status de visita agendada.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando a visita está marcada para ocorrer.
    /// </remarks>
    /// <value>1</value>
    [Display(Name = "Agendada")]
    Scheduled = 1,

    /// <summary>
    /// Status de visita finalizada.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando a visita foi concluída.
    /// </remarks>
    /// <value>2</value>
    [Display(Name = "Finalizada")]
    Done = 2,

    /// <summary>
    /// Status de visita cancelada.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando a visita foi cancelada.
    /// </remarks>
    /// <value>3</value>
    [Display(Name = "Cancelada")]
    Canceled = 3
}