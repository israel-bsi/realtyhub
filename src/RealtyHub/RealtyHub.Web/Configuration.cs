using MudBlazor;
using MudBlazor.Utilities;

namespace RealtyHub.Web;

public static class Configuration
{
    public const string HttpClientName = "realtyhyb";
    public static string BackendUrl { get; set; } = "http://localhost:5538";
    private const string PrincipalColor = "#B0C4DE";
    public static readonly MudTheme Theme = new()
    {
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = ["Raleway", "sans-serif"]
            }
        },
        Palette = new PaletteLight
        {
            //Primary = PrincipalColor, //Cor primária
            //PrimaryContrastText = new MudColor("#000000"), //Cor do texto do elemento primário
            //Secondary = Colors.LightBlue.Darken3, //Cor secundária
            //Background = Colors.Grey.Lighten4, //Cor de fundo da página
            //AppbarBackground = PrincipalColor, //Cor do appbar no topo da página
            //AppbarText = Colors.Shades.Black, //Cor do texto do appbar
            TextPrimary = Colors.Shades.Black, //Cor do texto primário
            DrawerText = Colors.Shades.Black, //Cor do texto do drawer
            //DrawerBackground = PrincipalColor
        },
        PaletteDark = new PaletteDark
        {
            //Primary = Colors.LightBlue.Accent3,
            //Secondary = Colors.LightBlue.Darken3,
            //AppbarBackground = PrincipalColor,
            //AppbarText = Colors.Shades.Black,
            //PrimaryContrastText = new MudColor("#000000"),
            //DrawerBackground = PrincipalColor,
            //DrawerText = Colors.Shades.Black,
            //TextPrimary = Colors.Shades.White
        }
    };
}