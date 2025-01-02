namespace RealtyHub.Core.Requests.Viewings;

public class RescheduleViewingRequest : Request
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
}