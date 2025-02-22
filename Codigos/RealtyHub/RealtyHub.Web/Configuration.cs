using MudBlazor;

namespace RealtyHub.Web;

public static class Configuration
{
    public const string HttpClientName = "realtyhub";
    public static string BackendUrl { get; set; } = "http://localhost:5538";
    public static string SrcLogo { get; set; } = SrcLogos.WhiteLogo;
    public static string GivenName { get; set; } = string.Empty;
    
    public static readonly MudTheme Theme = new()
    {
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = ["Raleway", "sans-serif"]
            }
        },
        PaletteLight = new PaletteLight
        {
            TextPrimary = Colors.Shades.Black,
            DrawerText = Colors.Shades.Black
        },
        PaletteDark = new PaletteDark()
    };

    public static class SrcLogos
    {
        public const string WhiteLogo = "/src/img/white-logo-nobg.png";
        public const string BlackLogo = "/src/img/black-logo-nobg.png";
    }
}