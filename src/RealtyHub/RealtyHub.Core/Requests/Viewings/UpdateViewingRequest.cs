using RealtyHub.Core.Enums;

namespace RealtyHub.Core.Requests.Viewings;

public class UpdateViewingRequest : Request
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public EViewingStatus ViewingStatus { get; set; }
    public long CustomerId { get; set; }
    public long PropertyId { get; set; }
}