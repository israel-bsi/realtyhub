namespace RealtyHub.ApiService;

public class Configuration
{
    public const string CorsPolicyName = "realtyhub";

    public static EmailConfiguration EmailSettings { get; set; } = new();

    public class EmailConfiguration
    {
        public string EmailFrom { get; set; } = "realtyhub.br@gmail.com";
        public string EmailPassword { get; set; } = string.Empty;
    }
}