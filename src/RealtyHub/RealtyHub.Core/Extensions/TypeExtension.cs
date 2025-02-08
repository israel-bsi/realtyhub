using System.Reflection;

namespace RealtyHub.Core.Extensions;

public static class TypeExtension
{
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