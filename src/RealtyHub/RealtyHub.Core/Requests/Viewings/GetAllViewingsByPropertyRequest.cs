namespace RealtyHub.Core.Requests.Viewings;

public class GetAllViewingsByPropertyRequest : Request
{
    public long PropertyId { get; set; }
}