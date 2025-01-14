using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

public interface IViaCepService
{
    Task<Response<ViaCepResponse?>> SearchAddressAsync(string cep);
    Task<Response<Address?>> GetAddressAsync(string cep);
}