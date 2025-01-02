using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

public interface IViaCepService
{
    Task<Response<ViaCepResponse?>> SearchAddressAsync(string cep);
}