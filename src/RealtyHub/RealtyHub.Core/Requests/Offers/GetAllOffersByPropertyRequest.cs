namespace RealtyHub.Core.Requests.Offers;

public class GetAllOffersByPropertyRequest : PagedRequest
{
    public long PropertyId { get; set; }
}