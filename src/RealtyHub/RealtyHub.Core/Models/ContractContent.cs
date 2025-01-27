using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Models;

public class ContractContent : Entity
{
    public string Id { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EContractModelType Type { get; set; }
    public bool ShowInPage { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Path => 
        $"{Configuration.BackendUrl}/contracts/{Id}{Extension}";
}