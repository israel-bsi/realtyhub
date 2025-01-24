using MudBlazor;
using System.Text.RegularExpressions;

namespace RealtyHub.Web;

public static class Utility
{
    public static Mask Masks { get; } = new();
    public class Mask
    {
        public readonly PatternMask Phone = new("(##) #########")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        public readonly PatternMask ZipCode = new("#####-###")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        public readonly PatternMask Cpf = new("###.###.###-##")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        public readonly PatternMask Cnpj = new("##.###.###/####-##")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        public readonly PatternMask Number = new("###")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };

        public readonly DateMask Date = new("dd/MM/yyyy");

        public readonly PatternMask Time = new("##:##")
        {
            MaskChars = [new MaskChar('#', @"[0-9]")]
        };
    }

    public static class Validations
    {
        public static readonly Regex TimeRegex = new(@"^(?:[01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);
    }

}