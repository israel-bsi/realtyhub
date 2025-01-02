using RealtyHub.Core.Models;
using RealtyHub.Core.Requests.Offers;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Handlers;

public interface IOfferHandler
{
    Task<Response<Offer?>> CreateAsync(CreateOfferRequest request);
    Task<Response<Offer?>> UpdateAsync(UpdateOfferRequest request);
    Task<Response<Offer?>> DeleteAsync(DeleteOfferRequest request);
    Task<Response<Offer?>> GetByIdAsync(GetOfferByIdRequest request);
    Task<Response<Offer?>> GetAllOffersByPropetyAsync(GetAllOffersByPropetyRequest request);
    Task<PagedResponse<List<Offer>?>> GetAllAsync(GetAllOffersRequest request);
}