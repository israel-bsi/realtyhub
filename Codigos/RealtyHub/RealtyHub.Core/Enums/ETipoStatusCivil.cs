using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

/// <summary>
/// Enumerador para os estados civis dos clientes.
/// </summary>
/// <remarks>
/// <para>O enum representa os diferentes estados civis que um cliente pode ter.</para>
/// <para>Cada valor do enum é associado a um número inteiro e possui um atributo
/// Display que fornece uma descrição legível para o estado civil.</para>
/// </remarks>
public enum ETipoStatusCivil
{
    /// <summary>
    /// Estado civil solteiro(a).
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o cliente é solteiro(a).
    /// </remarks>
    /// <value>1</value>
    [Display(Name = "Solteiro(a)")]
    Solteiro = 1,

    /// <summary>
    /// Estado civil casado(a).
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o cliente é casado(a).
    /// </remarks>
    /// <value>2</value>
    [Display(Name = "Casado(a)")]
    Casado = 2,

    /// <summary>
    /// Estado civil divorciado(a).
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o cliente é divorciado(a).
    /// </remarks>
    /// <value>3</value>
    [Display(Name = "Divorciado(a)")]
    Divorciado = 3,

    /// <summary>
    /// Estado civil viúvo(a).
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o cliente é viúvo(a).
    /// </remarks>
    /// <value>4</value>
    [Display(Name = "Viúvo(a)")]
    Viuvo = 4,

    /// <summary>
    /// Estado civil noivo(a).
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o cliente é noivo(a).
    /// </remarks>
    /// <value>5</value>
    [Display(Name = "Noivo(a)")]
    Noivo = 5
}