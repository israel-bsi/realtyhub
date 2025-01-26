using Microsoft.AspNetCore.Http;
using RealtyHub.Core.Models;

namespace RealtyHub.Core.Requests.ContractsContent;

public class UpdateContractContentRequest : Request
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HttpRequest? HttpRequest { get; set; }
    public List<FileData>? FileBytes { get; set; }
}