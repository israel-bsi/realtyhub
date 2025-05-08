using System.Reflection;

namespace RealtyHub.Core.Extensions;

/// <summary>
/// Métodos de extensão para a classe Type.
/// </summary>
/// <remarks>
/// Esta classe contém métodos de extensão para a classe Type, permitindo
/// realizar operações comuns de forma mais conveniente.
/// </remarks>
public static class TypeExtension
{
    /// <summary>
    /// Obtém uma propriedade aninhada de um tipo com base em um caminho de propriedade.
    /// </summary>
    /// <remarks>
    /// Este método obtém uma propriedade aninhada de um tipo com base em um caminho de propriedade.
    /// Se a propriedade não for encontrada, retorna null.
    /// </remarks>
    /// <param name="type">O tipo do qual obter a propriedade.</param>
    /// <param name="propertyPath">O caminho da propriedade aninhada.</param>
    /// <returns>A propriedade aninhada encontrada ou null se não for encontrada.</returns>
    public static PropertyInfo? GetNestedProperty(this Type type, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        var currentType = type;
        PropertyInfo? property = null;

        foreach (var prop in properties)
        {
            property = currentType.GetProperty(prop);
            if (property == null) return null;
            currentType = property.PropertyType;
        }
        return property;
    }
}