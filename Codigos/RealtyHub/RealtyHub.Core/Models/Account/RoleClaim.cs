namespace RealtyHub.Core.Models.Account;

/// <summary>
/// Representa uma declaração (claim) associada a uma role no ASP.NET Core Identity.
/// </summary>
/// <remarks>
/// Cada instância desta classe corresponde a um registro na tabela <c>AspNetRoleClaims</c>
/// e reflete a estrutura de um <see cref="System.Security.Claims.Claim"/> usada para autorização.
/// </remarks>
public class RoleClaim
{
    /// <summary>
    /// Obtém ou define o emissor desta claim.
    /// </summary>
    /// <value>
    /// Identificador da autoridade que emitiu a claim.
    /// </value>
    public string? Issuer { get; set; }

    /// <summary>
    /// Obtém ou define o emissor original desta claim.
    /// </summary>
    /// <value>
    /// Emissor inicial antes de qualquer reemissão ou transformação da claim.
    /// </value>
    public string? OriginalIssuer { get; set; }

    /// <summary>
    /// Obtém ou define o tipo desta claim.
    /// </summary>
    /// <value>
    /// Nome categórico da claim (por exemplo, <c>"permission"</c> ou <c>"role"</c>).
    /// </value>
    public string? Type { get; set; }

    /// <summary>
    /// Obtém ou define o valor desta claim.
    /// </summary>
    /// <value>
    /// Valor associado à claim (por exemplo, <c>"edit-articles"</c>).
    /// </value>
    public string? Value { get; set; }

    /// <summary>
    /// Obtém ou define o tipo de dado do valor desta claim.
    /// </summary>
    /// <value>
    /// Tipo de valor conforme <see cref="System.Security.Claims.ClaimValueTypes"/>,
    /// como <c>ClaimValueTypes.String</c>.
    /// </value>
    public string? ValueType { get; set; }
}
