using MudBlazor;
using System.Text.RegularExpressions;

namespace RealtyHub.Web;

/// <summary>
/// Classe utilitária que contém máscaras de entrada e validações comuns utilizadas na aplicação.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Propriedade que fornece as máscaras de entrada configuradas.
    /// </summary>
    public static Mask Masks { get; } = new();

    /// <summary>
    /// Classe que contém definições de máscaras de entrada para diversos formatos.
    /// </summary>
    public class Mask
    {
        /// <summary>
        /// Máscara para números de telefone no formato (##) #########.
        /// </summary>
        public readonly PatternMask Phone = new("(##) #########")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        /// <summary>
        /// Máscara para CEP no formato #####-###.
        /// </summary>
        public readonly PatternMask ZipCode = new("#####-###")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        /// <summary>
        /// Máscara para CPF no formato ###.###.###-##.
        /// </summary>
        public readonly PatternMask Cpf = new("###.###.###-##")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        /// <summary>
        /// Máscara para CNPJ no formato ##.###.###/####-##.
        /// </summary>
        public readonly PatternMask Cnpj = new("##.###.###/####-##")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        /// <summary>
        /// Máscara para números simples no formato ###.
        /// </summary>
        public readonly PatternMask Number = new("###")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };
    }

    /// <summary>
    /// Classe que contém expressões regulares para validações de entrada.
    /// </summary>
    public static class Validations
    {
        /// <summary>
        /// Expressão regular para validar o formato de horas (HH:mm).
        /// Aceita valores de 00:00 até 23:59.
        /// </summary>
        public static readonly Regex TimeRegex = new(@"^(?:[01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);
    }

}