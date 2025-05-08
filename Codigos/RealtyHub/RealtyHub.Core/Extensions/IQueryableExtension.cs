using System.Linq.Dynamic.Core;

namespace RealtyHub.Core.Extensions;

/// <summary>
/// Métodos de extensão para IQueryable
/// </summary>
/// <remarks>
/// Esta classe contém métodos de extensão para IQueryable, permitindo
/// realizar operações comuns de forma mais conveniente.
/// </remarks>
public static class IQueryableExtension
{
    /// <summary>
    /// Filtra uma IQueryable com base em um termo de pesquisa e uma propriedade.
    /// </summary>
    /// <remarks>
    /// Este método filtra uma IQueryable com base em um termo de pesquisa e uma propriedade.
    /// Se a propriedade não for encontrada ou o tipo não for suportado, retorna a IQueryable original.
    /// </remarks>
    /// <typeparam name="T">O tipo de entidade da IQueryable.</typeparam>
    /// <param name="query">A IQueryable a ser filtrada.</param>
    /// <param name="searchTerm">O termo de pesquisa a ser usado para filtrar.</param>
    /// <param name="filterBy">O nome da propriedade pela qual filtrar.</param>
    /// <returns>A IQueryable filtrada.</returns>
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