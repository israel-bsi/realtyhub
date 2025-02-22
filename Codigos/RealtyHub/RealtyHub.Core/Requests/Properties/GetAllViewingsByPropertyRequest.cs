namespace RealtyHub.Core.Requests.Properties;

public class GetAllViewingsByPropertyRequest : PagedRequest
{
    public long PropertyId { get; set; }
}