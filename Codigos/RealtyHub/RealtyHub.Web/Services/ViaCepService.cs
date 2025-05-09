﻿using RealtyHub.Core.Models;
using RealtyHub.Core.Responses;
using RealtyHub.Core.Services;
using System.Text.Json;

namespace RealtyHub.Web.Services;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Response<ViaCepResponse?>> SearchAddressAsync(string cep)
    {
        try
        {
            var url = $"https://viacep.com.br/ws/{cep}/json/";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var statusMessage = await response.Content.ReadAsStringAsync();
                return new Response<ViaCepResponse?>(null, (int)response.StatusCode, statusMessage);
            }

            var content = await response.Content.ReadAsStringAsync();
            var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(content);

            return new Response<ViaCepResponse?>(viaCepResponse);
        }
        catch (Exception ex)
        {
            return new Response<ViaCepResponse?>(null, 500, ex.Message);
        }
    }

    public async Task<Response<Address?>> GetAddressAsync(string cep)
    {
        try
        {
            var searchAddressAsync = await SearchAddressAsync(cep);
            if (searchAddressAsync is { IsSuccess: false, Data: null })
                return new Response<Address?>(null, 500, searchAddressAsync.Message);

            var address = new Address
            {
                ZipCode = cep,
                Street = searchAddressAsync.Data!.Street,
                Neighborhood = searchAddressAsync.Data.Neighborhood,
                City = searchAddressAsync.Data.City,
                State = searchAddressAsync.Data.State,
                Complement = searchAddressAsync.Data.Complement
            };

            return new Response<Address?>(address);
        }
        catch (Exception e)
        {
            return new Response<Address?>(null, 500, e.Message);
        }
    }
}