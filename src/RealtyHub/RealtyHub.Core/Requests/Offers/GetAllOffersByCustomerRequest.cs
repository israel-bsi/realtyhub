namespace RealtyHub.Core.Requests.Offers;

public class GetAllOffersByCustomerRequest : Request
{
    public long CustomerId { get; set; }
}