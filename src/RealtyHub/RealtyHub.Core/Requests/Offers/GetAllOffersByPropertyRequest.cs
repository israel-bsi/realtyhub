namespace RealtyHub.Core.Requests.Offers;

public class GetAllOffersByPropertyRequest : Request
{
    public long PropertyId { get; set; }
}