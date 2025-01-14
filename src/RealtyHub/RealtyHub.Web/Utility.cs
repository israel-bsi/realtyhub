using MudBlazor;

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
    }
}