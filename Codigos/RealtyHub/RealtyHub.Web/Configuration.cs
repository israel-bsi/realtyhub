using MudBlazor;

namespace RealtyHub.Web;

/// <summary>
/// Configurações globais da aplicação relativas ao backend, temas e logos.
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Nome do cliente HTTP configurado para a aplicação.
    /// </summary>
    public const string HttpClientName = "realtyhub";

    /// <summary>
    /// URL do backend da aplicação.
    /// </summary>
    public static string BackendUrl { get; set; } = "http://localhost:5538";

    /// <summary>
    /// Caminho da logo atualmente utilizada na aplicação.
    /// </summary>
    public static string SrcLogo { get; set; } = SrcLogos.WhiteLogo;

    /// <summary>
    /// Nome do usuário fornecido após a autenticação.
    /// </summary>
    public static string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// Tema da interface definido utilizando os componentes do MudBlazor.
    /// </summary>
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

    /// <summary>
    /// Define os caminhos para as logos disponíveis na aplicação.
    /// </summary>
    public static class SrcLogos
    {
        /// <summary>
        /// Caminho da logo branca sem fundo.
        /// </summary>
        public const string WhiteLogo = "/src/img/white-logo-nobg.png";

        /// <summary>
        /// Caminho da logo preta sem fundo.
        /// </summary>
        public const string BlackLogo = "/src/img/black-logo-nobg.png";
    }
}