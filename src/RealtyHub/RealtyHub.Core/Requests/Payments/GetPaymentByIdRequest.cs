namespace RealtyHub.Core.Requests.Payments;

public class GetPaymentByIdRequest : Request
{
    public long Id { get; set; }
}