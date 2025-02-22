using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RealtyHub.ApiService.Data;
using RealtyHub.Core.Utilities.FakeEntities;

namespace RealtyHub.ApiService.Common.Api;

public static class AppExtension
{
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.MapPost("v1/customers/createmany", async (
                AppDbContext context,
                [FromQuery] int individualQuantity = 0,
                [FromQuery] int bussinessQuantity = 0) =>
        {
            if (individualQuantity > 0)
            {
                var individualCustomers = CustomerFake
                    .GetFakeIndividualCustomers(individualQuantity);
                await context.Customers.AddRangeAsync(individualCustomers);
            }

            if (bussinessQuantity > 0)
            {
                var businessCustomers = CustomerFake
                    .GetFakeBusinessCustomers(bussinessQuantity);
                await context.Customers.AddRangeAsync(businessCustomers);

            }

            await context.SaveChangesAsync();

            return Results.Created();
        });

        app.MapPost("v1/properties/createmany", async (
            AppDbContext context,
            [FromQuery] int quantity = 0,
            [FromQuery] int customerId = 0,
            [FromQuery] int condominiumId = 0) =>
        {
            if (quantity > 0)
            {
                var properties = PropertyFake
                    .GetFakeProperties(quantity, customerId, condominiumId);
                await context.Properties.AddRangeAsync(properties);
            }

            await context.SaveChangesAsync();

            return Results.Created();
        });
    }
    public static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    public static void UseStaticFiles(this WebApplication app)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.PhotosPath),
            RequestPath = "/photos"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ContractsPath),
            RequestPath = "/contracts"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ContractTemplatesPath),
            RequestPath = "/contracts-templates"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Configuration.ReportsPath),
            RequestPath = "/reports"
        });
    }
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}