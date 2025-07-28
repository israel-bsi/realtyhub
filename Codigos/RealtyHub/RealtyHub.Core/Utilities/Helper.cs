using System.Text.RegularExpressions;

namespace RealtyHub.Core.Utilities;

/// <summary>
/// Classe de ajuda para formatação de documentos.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Formata uma string de números como CPF (xxx.xxx.xxx-xx) ou CNPJ (xx.xxx.xxx/xxxx-xx).
    /// </summary>
    /// <param name="documento">A string contendo os números do CPF ou CNPJ.</param>
    /// <returns>A string formatada ou a string original se não for um CPF/CNPJ válido.</returns>
    public static string FormatarCpfCnpj(string documento)
    {
        var numeros = Regex.Replace(documento, @"[^\d]", "");

        if (string.IsNullOrEmpty(numeros))
            return string.Empty;

        return numeros.Length switch
        {
            11 => Regex.Replace(numeros, @"^(\d{3})(\d{3})(\d{3})(\d{2})$", "$1.$2.$3-$4"),
            14 => Regex.Replace(numeros, @"^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$", "$1.$2.$3/$4-$5"),
            _ => numeros
        };
    }
}