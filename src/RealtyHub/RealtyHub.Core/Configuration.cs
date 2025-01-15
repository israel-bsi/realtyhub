namespace RealtyHub.Core;

public class Configuration
{
    public const int DefaultStatusCode = 200;
    public const int DefaultPageNumber = 1;
    public const string Locale = "pt_BR";
    public const int DefaultPageSize = 25;
    public static string ConnectionString { get; set; } = string.Empty;
    public static string BackendUrl { get; set; } = "http://localhost:5538";
    public static string FrontendUrl { get; set; } = "http://localhost:5187";
}