namespace RealtyHub.Core.Extensions;

/// <summary>
/// Métodos de extensão para a classe DateTime.
/// </summary>
/// <remarks>
/// Esta classe contém métodos de extensão para a classe DateTime, permitindo
/// realizar operações comuns de forma mais conveniente.
/// </remarks>
public static class DateTimeExtension
{
    /// <summary>
    /// Obtém o primeiro dia do mês de uma data específica.
    /// </summary>
    /// <remarks>
    /// Este método retorna o primeiro dia do mês de uma data específica.
    /// Se o ano e o mês não forem fornecidos, o ano e o mês da data serão usados.
    /// </remarks>
    /// <param name="date">A data para a qual o primeiro dia do mês será obtido.</param>
    /// <param name="year">O ano desejado. Se não for fornecido, o ano da data será usado.</param>
    /// <param name="month">O mês desejado. Se não for fornecido, o mês da data será usado.</param>
    /// <returns>O primeiro dia do mês especificado.</returns>
    public static DateTime GetFirstDay(this DateTime date, int? year = null, int? month = null)
    {
        return new(year ?? date.Year, month ?? date.Month, 1);
    }

    /// <summary>
    /// Obtém o último dia do mês de uma data específica.
    /// </summary>
    /// <remarks>
    /// Este método retorna o último dia do mês de uma data específica.
    /// Se o ano e o mês não forem fornecidos, o ano e o mês da data serão usados.
    /// </remarks>
    /// <param name="date">A data para a qual o último dia do mês será obtido.</param>
    /// <param name="year">O ano desejado. Se não for fornecido, o ano da data será usado.</param>
    /// <param name="month">O mês desejado. Se não for fornecido, o mês da data será usado.</param>
    /// <returns>O último dia do mês especificado.</returns>
    public static DateTime GetLastDay(this DateTime date, int? year = null, int? month = null)
    {
        return new DateTime(year ?? date.Year, month ?? date.Month, 1)
                .AddMonths(1)
                .AddDays(-1);
    }

    /// <summary>
    /// Converte uma data para uma string no formato UTC.
    /// </summary>
    /// <remarks>
    /// Este método converte uma data para uma string no formato UTC.
    /// Se a data for nula, retorna uma string vazia.
    /// </remarks>
    /// <param name="date">A data a ser convertida.</param>
    /// <returns>Uma string representando a data no formato UTC.</returns>
    public static string ToUtcString(this DateTime? date)
    {
        return date is null
            ? string.Empty
            : date.Value.ToUniversalTime().ToString("o");
    }

    /// <summary>
    /// Converte uma data para o fim do dia.
    /// </summary>
    /// <remarks>
    /// Este método converte uma data para o fim do dia, definindo a hora como 23:59:59.
    /// </remarks>
    /// <param name="date">A data a ser convertida.</param>
    /// <returns>A data convertida para o fim do dia.</returns>
    public static DateTime ToEndOfDay(this DateTime date)
    {
        return date.Date
            .AddHours(23)
            .AddMinutes(59)
            .AddSeconds(59);
    }
}