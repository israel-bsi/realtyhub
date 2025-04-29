using Microsoft.AspNetCore.Identity;

namespace RealtyHub.ApiService.Models;

/// <summary>
/// Representa um usuário do sistema.
/// </summary>
/// <remarks>
/// Esta classe herda de IdentityUser e adiciona propriedades específicas do domínio.
/// </remarks>
public class User : IdentityUser<long>
{
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    /// <value>Nome completo do usuário.</value>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// Identificador do Conselho Regional de Corretores de Imóveis (CRECI).
    /// </summary>
    /// <value>Identificador do Conselho Regional de Corretores de Imóveis.</value>
    public string Creci { get; set; } = string.Empty;

    /// <summary>
    /// Cargos do usuário no sistema.
    /// </summary>
    /// <value>Cargos do usuário no sistema.</value>
    public List<IdentityRole<long>>? Roles { get; set; }
}