﻿using RealtyHub.ApiService.Data;
using RealtyHub.Core.Utilities.FakeEntities;
using System.Net.Http.Json;

namespace RealtyHub.Tests;

public class MockData
{
    public static async Task CreateCustomers(RealtyHubApiTests application,
        bool create, int quantityBusiness, int quantityIndividual)
    {
        using var scope = application.Services.CreateScope();
        var provider = scope.ServiceProvider;
        await using var dbContext = provider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        if (create)
        {
            var customersBusinessToCreate = CustomerFake.GetFakeBusinessCustomers(quantityBusiness);
            var customersIndividualToCreate = CustomerFake.GetFakeIndividualCustomers(quantityIndividual);
            await dbContext.Customers.AddRangeAsync(customersBusinessToCreate);
            await dbContext.Customers.AddRangeAsync(customersIndividualToCreate);
            await dbContext.SaveChangesAsync();
        }
    }
    public static async Task<HttpClient> CreateClient(RealtyHubApiTests application)
    {
        var client = application.CreateClient();

        const string registerUrl = "/v1/identity/register";
        const string loginUrl = "/v1/identity/login?useCookies=true&useSessionCookies=true";

        var login = new Login("israel@gmail.com", "!W92X+!Q@rOwC48+v.V3");

        var registerResponse = await client.PostAsJsonAsync(registerUrl, login);
        var loginResponse = await client.PostAsJsonAsync(loginUrl, login);

        return client;
    }
    private record Login
    {
        public Login(string Email, string Password)
        {
            email = Email;
            password = Password;
        }

        public string email { get; init; }
        public string password { get; init; }

        public void Deconstruct(out string email, out string password)
        {
            email = this.email;
            password = this.password;
        }
    }
}