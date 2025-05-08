using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RealtyHub.Core.Extensions;
/// <summary>
/// Métodos de extensão para a classe Enum.
/// </summary>
/// <remarks>
/// Esta classe contém métodos de extensão para a classe Enum, permitindo
/// obter o nome de exibição associado a um valor de enumeração.
/// </remarks>
public static class EnumExtension
{
    /// <summary>
    /// Obtém o nome de exibição associado a um valor de enumeração.
    /// </summary>
    /// <remarks>
    /// Este método retorna o nome de exibição associado a um valor de enumeração,
    /// utilizando o atributo DisplayAttribute, se estiver presente.
    /// Caso contrário, retorna o nome do valor da enumeração como string.
    /// </remarks>
    /// <param name="value">O valor da enumeração para o qual o nome de exibição será obtido.</param>
    /// <returns>
    /// O nome de exibição associado ao valor da enumeração, ou o nome do valor
    /// da enumeração como string, se o atributo DisplayAttribute não estiver presente.
    /// </returns>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? value.ToString();
    }
}