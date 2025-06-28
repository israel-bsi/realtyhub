using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;

namespace RealtyHub.Core.Services;

/// <summary>
/// Define os métodos para buscar informações de endereço via CEP utilizando a API ViaCep.
/// </summary>
public interface IViaCepService
{
    /// <summary>
    /// Busca o endereço associado ao CEP especificado utilizando a API ViaCep.
    /// </summary>
    /// <param name="cep">O CEP a ser pesquisado.</param>
    /// <returns>
    /// Um objeto <c><see cref="Response{T}"/></c> contendo um 
    /// <c><see cref="ViaCepResponse"/></c> com as informações do endereço, se encontrado.
    /// </returns>
    Task<Response<ViaCepResponse?>> SearchAddressAsync(string cep);

    /// <summary>
    /// Obtém o endereço a partir do CEP especificado, retornando um objeto do tipo <c><see cref="Endereco"/></c>.
    /// </summary>
    /// <param name="cep">O CEP a ser pesquisado.</param>
    /// <returns>
    /// Um objeto <c><see cref="Response{T}"/></c> contendo um <c><see cref="Endereco"/></c> com 
    /// as informações do endereço, se encontrado.
    /// </returns>
    Task<Response<Endereco?>> GetAddressAsync(string cep);
}