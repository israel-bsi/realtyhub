using System.Linq.Dynamic.Core;

namespace RealtyHub.Core.Extensions;

public static class IQueryableExtension
{
    public static IQueryable<T> FilterByProperty<T>(this IQueryable<T> query, string searchTerm, string filterBy)
    {
        var propertyInfo = typeof(T).GetNestedProperty(filterBy);
        var propertyType = propertyInfo?.PropertyType;

        if (propertyType == null) return query;

        if (propertyType == typeof(string))
        {
            query = query.Where($"{filterBy}.ToLower().Contains(@0.ToLower())", searchTerm);
        }
        else if (propertyType == typeof(bool))
        {
            var boolValue = bool.TryParse(searchTerm, out var parsedBool) && parsedBool;
            query = query.Where($"{filterBy} == @0", !boolValue);
        }
        else if (propertyType == typeof(int))
        {
            searchTerm = string.IsNullOrEmpty(searchTerm) ? "0" : searchTerm;
            var searchValue = Convert.ChangeType(searchTerm, propertyType);
            query = query.Where($"{filterBy} == @0", searchValue);
        }
        return query;
    }
}