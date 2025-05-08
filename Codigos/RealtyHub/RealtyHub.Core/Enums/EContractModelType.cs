using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Core.Enums;

/// <summary>
/// Enumerador para os tipos de contrato.
/// </summary>
/// <remarks>
/// <para>O enum representa os diferentes tipos de contrato que podem existir.</para>
/// <para>Cada valor do enum é associado a um número inteiro e possui um atributo 
/// Display que fornece uma descrição legível para o tipo de contrato.</para>
/// </remarks>
public enum EContractModelType
{

    /// <summary>
    /// Tipo de contrato não definido.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o tipo de contrato não é especificado.
    /// </remarks>
    /// <value>0</value>
    [Display(Name = "Não definido")]
    None,

    /// <summary>
    /// Tipo de contrato entre pessoa jurídica e pessoa jurídica.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o contrato é entre duas pessoas jurídicas.
    /// </remarks>
    /// <value>1</value>
    [Display(Name = "Pessoa Jurídica para Pessoa Jurídica")]
    PJPJ = 1,

    /// <summary>
    /// Tipo de contrato entre pessoa física e pessoa física.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o contrato é entre duas pessoas físicas.
    /// </remarks>
    /// <value>2</value>
    [Display(Name = "Pessoa Física para Pessoa Física")]
    PFPF = 2,

    /// <summary>
    /// Tipo de contrato entre pessoa física e pessoa jurídica.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o contrato é entre uma pessoa física e uma pessoa jurídica.
    /// </remarks>
    /// <value>3</value>
    [Display(Name = "Pessoa Física para Pessoa Jurídica")]
    PFPJ = 3,

    /// <summary>
    /// Tipo de contrato entre pessoa jurídica e pessoa física.
    /// </summary>
    /// <remarks>
    /// Este valor é utilizado quando o contrato é entre uma pessoa jurídica e uma pessoa física.
    /// </remarks>
    /// <value>4</value>
    [Display(Name = "Pessoa Jurídica para Pessoa Física")]
    PJPF = 4
}