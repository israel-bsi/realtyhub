namespace RealtyHub.ApiService;

public class Configuration
{
    public const string CorsPolicyName = "realtyhub";
    public static string ContractsPath { get; set; } = string.Empty;
    public static string PhotosPath { get; set; } = string.Empty;
    public static string ContractTemplatesPath { get; set; } = string.Empty;
    public static string ReportsPath { get; set; } = string.Empty;
    public static string LogosPath { get; set; } = string.Empty;

    public static EmailConfiguration EmailSettings { get; set; } = new();
    public class EmailConfiguration
    {
        public string EmailFrom { get; set; } = "realtyhub.br@gmail.com";
        public string EmailPassword { get; set; } = string.Empty;
    }

    
}