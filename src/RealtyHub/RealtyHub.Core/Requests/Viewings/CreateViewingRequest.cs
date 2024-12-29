using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Requests.Viewings;

public class CreateViewingRequest : Request
{
    public DateTime Date { get; set; }
    public EViewingStatus ViewingStatus { get; set; }
    public long CustomerId { get; set; }
    public long PropertyId { get; set; }
}