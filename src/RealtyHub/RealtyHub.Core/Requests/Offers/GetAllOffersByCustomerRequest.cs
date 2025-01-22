namespace RealtyHub.Core.Requests.Offers;

public class GetAllOffersByCustomerRequest : PagedRequest
{
    public long CustomerId { get; set; }
}