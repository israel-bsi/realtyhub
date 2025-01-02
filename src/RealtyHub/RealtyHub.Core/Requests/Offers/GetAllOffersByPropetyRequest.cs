namespace RealtyHub.Core.Requests.Offers;

public class GetAllOffersByPropetyRequest : Request
{
    public long Id { get; set; }
    public long IdProperty { get; set; }
}